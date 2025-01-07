using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Laverie.Domain.Entities;
using Laverie.API.Infrastructure.context;
using MySql.Data.MySqlClient;
using Laverie.Domain.DTOS;
using Microsoft.AspNetCore.Mvc;

public class ConfigurationRepo
{
    private readonly AppDbContext _dbContext;

    public ConfigurationRepo(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<User>> GetConfigAsync()
    {
        var users = new List<User>();

        using (var connection = (MySqlConnection)_dbContext.CreateConnection())
        {
            await connection.OpenAsync();

            string query = @"
            SELECT 
                p.Id AS ProprietaireId, 
                p.Name AS ProprietaireName, 
                p.Email AS ProprietaireEmail, 
                l.Id AS LaverieId, 
                l.NomLaverie AS LaverieName,
                l.Address AS LaverieAddress,
                l.Latitude AS LaverieLatitude,
                l.Longitude AS LaverieLongitude,
                l.Services AS LaverieServices,
                m.Id AS MachineId, 
                m.Type AS MachineType, 
                m.Status AS MachineStatus, 
                c.Id AS CycleId, 
                c.Price AS CyclePrice, 
                c.CycleDuration AS CycleDuration,
                c.MachineId AS MachineId,
                a.Id AS ActionId,
                a.CycleId AS cycleActionId,
                a.StartTime AS StartTime
            FROM 
                Proprietaire p
            LEFT JOIN 
                Laverie l ON p.Id = l.ProprietaireId
            LEFT JOIN 
                Machine m ON l.Id = m.LaverieId
            LEFT JOIN 
                Cycle c ON m.Id = c.MachineId
            LEFT JOIN 
                Action a ON c.Id = a.CycleId
            ORDER BY 
                p.Id, l.Id, m.Id, c.Id, a.Id";

            using (var command = new MySqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                User currentProprietaire = null;

                while (await reader.ReadAsync())
                {
                    int proprietaireId = reader.GetInt32("ProprietaireId");

                    if (currentProprietaire == null || currentProprietaire.id != proprietaireId)
                    {
                        currentProprietaire = new User
                        {
                            id = proprietaireId,
                            name = reader.GetString("ProprietaireName"),
                            email = reader.GetString("ProprietaireEmail"),
                            laundries = new List<Laundry>()
                        };
                        users.Add(currentProprietaire);
                    }

                    int? laverieId = reader.IsDBNull("LaverieId") ? (int?)null : reader.GetInt32("LaverieId");
                    if (laverieId.HasValue)
                    {
                        var laverie = currentProprietaire.laundries
                            .Find(l => l.id == laverieId.Value) ?? new Laundry
                            {
                                id = laverieId.Value,
                                nomLaverie = reader.GetString("LaverieName"),
                                address = reader.IsDBNull("LaverieAddress") ? null : reader.GetString("LaverieAddress"),
                                Latitude = reader.IsDBNull("LaverieLatitude") ? 0 : (float)reader.GetDouble("LaverieLatitude"),
                                Longitude = reader.IsDBNull("LaverieLongitude") ? 0 : (float)reader.GetDouble("LaverieLongitude"),
                                Services = reader.IsDBNull("LaverieServices") ? new List<string>() : reader.GetString("LaverieServices").Split(',').ToList(),
                                machines = new List<Machine>()
                            };

                        if (!currentProprietaire.laundries.Contains(laverie))
                        {
                            currentProprietaire.laundries.Add(laverie);
                        }

                        int? machineId = reader.IsDBNull("MachineId") ? (int?)null : reader.GetInt32("MachineId");
                        if (machineId.HasValue)
                        {
                            var machine = laverie.machines
                                .Find(m => m.id == machineId.Value) ?? new Machine
                                {
                                    id = machineId.Value,
                                    type = reader.GetString("MachineType"),
                                    status = reader.GetBoolean("MachineStatus"),
                                    cycles = new List<Cycle>()
                                };

                            if (!laverie.machines.Contains(machine))
                            {
                                laverie.machines.Add(machine);
                            }

                            int? cycleId = reader.IsDBNull("CycleId") ? (int?)null : reader.GetInt32("CycleId");
                            if (cycleId.HasValue)
                            {
                                var cycle = new Cycle
                                {
                                    id = cycleId.Value,
                                    price = reader.GetDecimal("CyclePrice"),
                                    cycleDuration = reader.GetString("CycleDuration"),
                                    machineId = reader.GetInt32("MachineId"),
                                    transactions = new List<Laverie.Domain.Entities.Action>()
                                };

                                if (!machine.cycles.Any(c => c.id == cycle.id))
                                {
                                    machine.cycles.Add(cycle);
                                }

                                int? actionId = reader.IsDBNull("ActionId") ? (int?)null : reader.GetInt32("ActionId");
                                if (actionId.HasValue)
                                {
                                    var action = new Laverie.Domain.Entities.Action
                                    {
                                        Id = actionId.Value,
                                        CycleId = reader.GetInt32("cycleActionId"),
                                        StartTime = reader.GetDateTime("StartTime"),
                                    };

                                    if (!cycle.transactions.Any(c => c.Id == action.Id))
                                    {
                                        cycle.transactions.Add(action);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return users;
    }

    public async Task<bool> StartMachine(int MachineId, int IdCycle)
    {
        using (var connection = (MySqlConnection)_dbContext.CreateConnection())
        {
            await connection.OpenAsync();

            using (var transaction = await connection.BeginTransactionAsync()) 
            {
                try
                {
                    string toggleStatusQuery = @"
                    UPDATE Machine 
                    SET Status = @NewStatus 
                    WHERE Id = @MachineId";

                    using (var toggleStatusCommand = new MySqlCommand(toggleStatusQuery, connection, transaction))
                    {
                        toggleStatusCommand.Parameters.AddWithValue("@NewStatus", true);
                        toggleStatusCommand.Parameters.AddWithValue("@MachineId", MachineId);

                        int rowsAffected = await toggleStatusCommand.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            DateTime startTime = DateTime.Now;
                            string insertActionQuery = @"
                            INSERT INTO Action (CycleId, StartTime)
                            VALUES (@CycleId, @StartTime)";

                            using (var insertActionCommand = new MySqlCommand(insertActionQuery, connection, transaction))
                            {
                                insertActionCommand.Parameters.AddWithValue("@CycleId", IdCycle);
                                insertActionCommand.Parameters.AddWithValue("@StartTime", startTime);

                                int actionRowsAffected = await insertActionCommand.ExecuteNonQueryAsync();

                                if (actionRowsAffected > 0)
                                {
                                    await transaction.CommitAsync(); 
                                    return true;
                                }
                            }
                        }
                        await transaction.RollbackAsync(); 
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(); 
                    throw new Exception("Error starting machine: " + ex.Message);
                }
            }
        }
    }
    public async Task<bool> StopMachine(int MachineId)
    {
        using (var connection = (MySqlConnection)_dbContext.CreateConnection())
        {
            await connection.OpenAsync();

            using (var transaction = await connection.BeginTransactionAsync()) 
            {
                try
                {
                    string toggleStatusQuery = @"
                    UPDATE Machine 
                    SET Status = @NewStatus 
                    WHERE Id = @MachineId";

                    using (var toggleStatusCommand = new MySqlCommand(toggleStatusQuery, connection, transaction))
                    {
                        toggleStatusCommand.Parameters.AddWithValue("@NewStatus", false);
                        toggleStatusCommand.Parameters.AddWithValue("@MachineId", MachineId);

                        int rowsAffected = await toggleStatusCommand.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            await transaction.CommitAsync(); 
                            return true;
                        }
                        await transaction.RollbackAsync(); 
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(); 
                    throw new Exception("Error stopping machine: " + ex.Message);
                }
            }
        }
    }

    public async Task<int> AddCycleAsync(CycleCreationDTO cycle)
    {
        using (var connection = (MySqlConnection)_dbContext.CreateConnection())
        {
            await connection.OpenAsync();
            using (var transaction = await connection.BeginTransactionAsync())
            {
                try
                {
                    // Insert the cycle
                    string insertCycleQuery = @"
                    INSERT INTO cycle (Price, MachineId, CycleDuration) 
                    VALUES (@Price, @MachineId, @CycleDuration);
                    SELECT LAST_INSERT_ID();"; // This returns the last inserted ID

                    using (var insertCycleCommand = new MySqlCommand(insertCycleQuery, connection, transaction))
                    {
                        insertCycleCommand.Parameters.AddWithValue("@Price", cycle.price);
                        insertCycleCommand.Parameters.AddWithValue("@MachineId", cycle.machineId);
                        insertCycleCommand.Parameters.AddWithValue("@CycleDuration", cycle.cycleDuration);

                        // Execute the insert query and retrieve the last inserted ID
                        int cycleId = Convert.ToInt32(await insertCycleCommand.ExecuteScalarAsync());

                        if (cycleId > 0)
                        {
                            await transaction.CommitAsync(); // Commit the transaction
                            return cycleId; // Return the ID of the inserted cycle
                        }

                        await transaction.RollbackAsync(); // Rollback if no rows were inserted
                        return 0; // Return 0 if insertion failed
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(); // Rollback on exception
                    throw; // Re-throw the exception to handle it upstream
                }
            }
        }
    }


}
