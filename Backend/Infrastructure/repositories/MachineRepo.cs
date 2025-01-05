using Laverie.API.Infrastructure.context;
using Laverie.Domain.DTOS;
using Laverie.Domain.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

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
        public List<Machine> GetAll()
        {
            var machines = new List<Machine>();
            using (var conn = (MySqlConnection)_dbContext.CreateConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM machine", conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    machines.Add(new Machine
                    {
                        id = reader.GetInt32("id"),
                        status = reader.GetBoolean("status"),
                        type = reader.GetString("type")
                    });
                }
            }
            return machines;
        }

        // Get machine by Id
        public Machine GetById(int id)
        {
            Machine machine = null;
            using (var conn = (MySqlConnection)_dbContext.CreateConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM machine WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    machine = new Machine
                    {
                        id = reader.GetInt32("id"),
                        status = reader.GetBoolean("status"),
                        type = reader.GetString("type")
                    };
                }
            }
            return machine;
        }

        // Add a new machine
        public bool Create(MachineCreationDTO machine)
        {
            try
            {
                using (var conn = (MySqlConnection)_dbContext.CreateConnection())
                {
                    conn.Open();

                    // Insert query with parameters
                    MySqlCommand cmd = new MySqlCommand(
                        "INSERT INTO machine (status, type, LaverieId) " +
                        "VALUES (@status, @type, @LaverieId)", conn);

                    // Add parameters
                    cmd.Parameters.AddWithValue("@status", machine.status);
                    cmd.Parameters.AddWithValue("@type", machine.type);
                    cmd.Parameters.AddWithValue("@LaverieId", machine.LaverieId);

                    // Execute query and check rows affected
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Error logging (you may want to log this to a file or external service)
                throw new Exception($"An error occurred while creating the machine: {ex.Message}", ex);
            }
        }

        // Update an existing machine
        public bool Update(MachineUpdateDTO machine, int id)
        {
            using (var conn = (MySqlConnection)_dbContext.CreateConnection())
            {
                conn.Open();

                // Check if machine exists before updating
                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM machine WHERE id = @id", conn);
                checkCmd.Parameters.AddWithValue("@id", id);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    return false; 
                }

                
                MySqlCommand cmd = new MySqlCommand(
                    "UPDATE machine SET status = @status, type = @type WHERE id = @id", conn);

                
                cmd.Parameters.AddWithValue("@status", machine.status);
                cmd.Parameters.AddWithValue("@type", machine.type);
                cmd.Parameters.AddWithValue("@id", id);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // Delete a machine
        public bool Delete(int id)
        {
            using (var conn = (MySqlConnection)_dbContext.CreateConnection())
            {
                conn.Open();

                // Check if the machine exists before deleting
                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM machine WHERE id = @id", conn);
                checkCmd.Parameters.AddWithValue("@id", id);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    return false; // Machine doesn't exist
                }

                // Proceed with deletion
                MySqlCommand cmd = new MySqlCommand("DELETE FROM machine WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
    }
}
