using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Laverie.Domain.Entities;
using Laverie.API.Infrastructure.context;
using Laverie.Domain.DTOS;

namespace Laverie.API.Infrastructure.repositories
{
    public class LaundryRepo
    {
        private readonly AppDbContext _dbContext;

        public LaundryRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get all laundries
        public List<Laundry> GetAll()
        {
            var laundries = new List<Laundry>();
            using (var conn = (MySqlConnection)_dbContext.CreateConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM laverie", conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    laundries.Add(new Laundry
                    {
                        id = reader.GetInt32("Id"),
                        nomLaverie = reader.GetString("NomLaverie"),
                        address = reader.GetString("Address"),
                        Latitude = reader.GetFloat("Latitude"),
                        Longitude = reader.GetFloat("Longitude"),
                        Services = reader.GetString("Services").Split(',').ToList()
                    });
                }
            }
            return laundries;
        }

        // Get laundry by Id
        public Laundry GetById(int id)
        {
            Laundry laundry = null;
            using (var conn = (MySqlConnection)_dbContext.CreateConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM laverie WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    laundry = new Laundry
                    {
                        id = reader.GetInt32("Id"),
                        nomLaverie = reader.GetString("NomLaverie"),
                        address = reader.GetString("Address"),
                        Latitude = reader.GetFloat("Latitude"),
                        Longitude = reader.GetFloat("Longitude"),
                        Services = reader.GetString("Services").Split(',').ToList()
                    };
                }
            }
            return laundry;
        }

        public bool Create(LaundryCreationDTO laundry)
        {
            try
            {
                using (var conn = (MySqlConnection)_dbContext.CreateConnection())
                {
                    conn.Open();

                    // Insert query with parameters
                    MySqlCommand cmd = new MySqlCommand(
                        " INSERT INTO laverie " +
                        " (NomLaverie, Address, Latitude, ProprietaireId," +
                        " Longitude, Services) " +
                        " VALUES (@NomLaverie, @Address, @Latitude, @ProprietaireId," +
                        " @Longitude, @Services)", conn);

                    // Add parameters
                    cmd.Parameters.AddWithValue("@NomLaverie", laundry.nomLaverie);
                    cmd.Parameters.AddWithValue("@Address", laundry.address);
                    cmd.Parameters.AddWithValue("@Latitude", laundry.Latitude);
                    cmd.Parameters.AddWithValue("@ProprietaireId", laundry.ProprietaireId);
                    cmd.Parameters.AddWithValue("@Longitude", laundry.Longitude);
                    cmd.Parameters.AddWithValue("@Services", string.Join(",", laundry.Services));

                    // Execute query and return the number of rows affected
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0 ;
                }
            }
            catch (Exception ex)
            {
                // Error logging (you may want to log this to a file or external service)
                throw new Exception($"An error occurred while creating the laundry: {ex.Message}", ex);
            }
        }




        // Update an existing laundry
        public bool Update(LaundryUpdateDTO laundry, int id)
        {
            using (var conn = (MySqlConnection)_dbContext.CreateConnection())
            {
                conn.Open();

                // Check if laundry exists before updating
                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM laverie WHERE Id = @Id", conn);
                checkCmd.Parameters.AddWithValue("@Id", id);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    return false; // Laundry doesn't exist
                }

                // Proceed with the update
                MySqlCommand cmd = new MySqlCommand(
                    "UPDATE laverie SET NomLaverie = @NomLaverie, Address = @Address," +
                    " Latitude = @Latitude, Longitude = @Longitude, Services = @Services " +
                    "WHERE Id = @Id", conn);

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@NomLaverie", laundry.nomLaverie);
                cmd.Parameters.AddWithValue("@Address", laundry.address);
                cmd.Parameters.AddWithValue("@Latitude", laundry.Latitude);
                cmd.Parameters.AddWithValue("@Longitude", laundry.Longitude);
                cmd.Parameters.AddWithValue("@Services", string.Join(",", laundry.Services));

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }


        // Delete a laundry
        public bool Delete(int id)
        {
            using (var conn = (MySqlConnection)_dbContext.CreateConnection())
            {
                conn.Open();

                // Check if the laundry exists before deleting
                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM laverie WHERE Id = @Id", conn);
                checkCmd.Parameters.AddWithValue("@Id", id);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    return false; // Laundry doesn't exist
                }

                // Proceed with deletion
                MySqlCommand cmd = new MySqlCommand("DELETE FROM laverie WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

    }
}
