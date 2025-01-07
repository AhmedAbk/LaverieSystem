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

      
        public bool Create(MachineCreationDTO machine)
        {
            try
            {
                using (var conn = (MySqlConnection)_dbContext.CreateConnection())
                {
                    conn.Open();

                    
                    MySqlCommand cmd = new MySqlCommand(
                        "INSERT INTO machine (status, type, LaverieId) " +
                        "VALUES (@status, @type, @LaverieId)", conn);

                   
                    cmd.Parameters.AddWithValue("@status", machine.status);
                    cmd.Parameters.AddWithValue("@type", machine.type);
                    cmd.Parameters.AddWithValue("@LaverieId", machine.LaverieId);

                   
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                
                throw new Exception($"An error occurred while creating the machine: {ex.Message}", ex);
            }
        }

        
        public bool Update(MachineUpdateDTO machine, int id)
        {
            using (var conn = (MySqlConnection)_dbContext.CreateConnection())
            {
                conn.Open();

              
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

    
        public bool Delete(int id)
        {
            using (var conn = (MySqlConnection)_dbContext.CreateConnection())
            {
                conn.Open();

                
                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM machine WHERE id = @id", conn);
                checkCmd.Parameters.AddWithValue("@id", id);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    return false; 
                }

              
                MySqlCommand cmd = new MySqlCommand("DELETE FROM machine WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
    }
}
