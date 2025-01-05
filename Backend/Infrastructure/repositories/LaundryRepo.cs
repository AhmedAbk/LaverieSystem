using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Laverie.Domain.Entities;
using Laverie.API.Infrastructure.context;

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

        // Create a new laundry
        public void Create(Laundry laundry)
        {
            using (var conn = (MySqlConnection)_dbContext.CreateConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    "INSERT INTO laverie (NomLaverie, Address, Latitude, Longitude, Services) VALUES (@NomLaverie, @Address, @Latitude, @Longitude, @Services)", conn);
                cmd.Parameters.AddWithValue("@NomLaverie", laundry.nomLaverie);
                cmd.Parameters.AddWithValue("@Address", laundry.address);
                cmd.Parameters.AddWithValue("@Latitude", laundry.Latitude);
                cmd.Parameters.AddWithValue("@Longitude", laundry.Longitude);
                cmd.Parameters.AddWithValue("@Services", string.Join(",", laundry.Services));
                cmd.ExecuteNonQuery();
            }
        }

        // Update an existing laundry
        public void Update(Laundry laundry)
        {
            using (var conn = (MySqlConnection)_dbContext.CreateConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    "UPDATE laverie SET NomLaverie = @NomLaverie, Address = @Address, Latitude = @Latitude, Longitude = @Longitude, Services = @Services WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", laundry.id);
                cmd.Parameters.AddWithValue("@NomLaverie", laundry.nomLaverie);
                cmd.Parameters.AddWithValue("@Address", laundry.address);
                cmd.Parameters.AddWithValue("@Latitude", laundry.Latitude);
                cmd.Parameters.AddWithValue("@Longitude", laundry.Longitude);
                cmd.Parameters.AddWithValue("@Services", string.Join(",", laundry.Services));
                cmd.ExecuteNonQuery();
            }
        }

        // Delete a laundry
        public void Delete(int id)
        {
            using (var conn = (MySqlConnection)_dbContext.CreateConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM Laundry WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
