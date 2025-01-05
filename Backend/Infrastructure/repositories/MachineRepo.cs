using Laverie.API.Infrastructure.context;
using Laverie.Domain.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Laverie.API.Infrastructure.repositories
{
    public class MachineRepo
    {
        private readonly AppDbContext _dbContext;

        public MachineRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get all machines
        public async Task<List<Machine>> GetAllMachinesAsync()
        {
            var machines = new List<Machine>();

            using (var connection = (MySqlConnection)_dbContext.CreateConnection())
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM Machines", (MySqlConnection)connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var machine = new Machine
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            status = reader.GetBoolean(reader.GetOrdinal("status")),
                            type = reader.GetString(reader.GetOrdinal("type"))
                        };
                        machines.Add(machine);
                    }
                }
            }
            return machines;
        }

        // Get machine by ID
        public async Task<Machine> GetMachineByIdAsync(int id)
        {
            Machine machine = null;

            using (var connection = (MySqlConnection)_dbContext.CreateConnection())
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM Machines WHERE id = @id", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        machine = new Machine
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            status = reader.GetBoolean(reader.GetOrdinal("status")),
                            type = reader.GetString(reader.GetOrdinal("type"))
                        };
                    }
                }
            }
            return machine;
        }

        // Add a new machine
        public async Task<Machine> AddMachineAsync(Machine machine)
        {
            using (var connection = (MySqlConnection)_dbContext.CreateConnection())
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("INSERT INTO Machines (status, type) VALUES (@status, @type)", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@status", machine.status);
                command.Parameters.AddWithValue("@type", machine.type);

                await command.ExecuteNonQueryAsync();

                // Optionally, retrieve the last inserted ID if you need it
                machine.id = (int)command.LastInsertedId;
            }
            return machine;
        }

        // Update machine details
        public async Task<bool> UpdateMachineAsync(Machine machine)
        {
            using (var connection = (MySqlConnection)_dbContext.CreateConnection())
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("UPDATE Machines SET status = @status, type = @type WHERE id = @id", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@status", machine.status);
                command.Parameters.AddWithValue("@type", machine.type);
                command.Parameters.AddWithValue("@id", machine.id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        // Delete machine
        public async Task<bool> DeleteMachineAsync(int id)
        {
            using (var connection = (MySqlConnection)_dbContext.CreateConnection())
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("DELETE FROM Machines WHERE id = @id", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@id", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }
    }
}
