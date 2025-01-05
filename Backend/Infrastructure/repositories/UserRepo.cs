using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Laverie.Domain.Entities;
using Laverie.API.Infrastructure.context;
using MySql.Data.MySqlClient;
using Laverie.Domain.DTOS;

namespace Laverie.API.Infrastructure.repositories
{
    public class UserRepo
    {
        private readonly AppDbContext _dbContext;

        public UserRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<User> GetAll()
        {
            var proprietaires = new List<User>();
            using (var conn = (MySqlConnection)_dbContext.CreateConnection()) // Use CreateConnection here
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Proprietaire", conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    proprietaires.Add(new User
                    {
                        id = reader.GetInt32("Id"),
                        name = reader.GetString("Name"),
                        email = reader.GetString("Email"),
                        password = reader.GetString("Password"),
                        age = reader.GetInt32("Age")
                    });
                }
            }
            return proprietaires;
        }

        public User GetById(int id)
        {
            User proprietaire = null;
            using (var conn = (MySqlConnection)_dbContext.CreateConnection()) // Use CreateConnection here
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Proprietaire WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    proprietaire = new User
                    {
                        id = reader.GetInt32("Id"),
                        name = reader.GetString("Name"),
                        email = reader.GetString("Email"),
                        password = reader.GetString("Password"),
                        age = reader.GetInt32("Age")
                    };
                }
            }
            return proprietaire;
        }

        public void Create(UserCreationDTO proprietaire)
        {
            using (var conn = (MySqlConnection)_dbContext.CreateConnection()) // Use CreateConnection here
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    "INSERT INTO proprietaire (Name, Email, Password, Age) " +
                    "VALUES (@Name, @Email, @Password, @Age)", conn);
                cmd.Parameters.AddWithValue("@Name", proprietaire.Name);
                cmd.Parameters.AddWithValue("@Email", proprietaire.Email);
                cmd.Parameters.AddWithValue("@Password", proprietaire.Password);
                cmd.Parameters.AddWithValue("@Age", proprietaire.Age);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    // Handle duplicate entry error or other database exceptions
                    if (ex.Number == 1062) // Duplicate entry error code
                    {
                        throw new Exception("Email already exists.");
                    }
                    throw;
                }
            }
        }

        public void Update(UserCreationDTO proprietaire, int id)
        {
            using (var conn = (MySqlConnection)_dbContext.CreateConnection()) // Use CreateConnection here
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    "UPDATE proprietaire SET Name = @Name, Email = @Email, Password = @Password, Age = @Age WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Name", proprietaire.Name);
                cmd.Parameters.AddWithValue("@Email", proprietaire.Email);
                cmd.Parameters.AddWithValue("@Password", proprietaire.Password);
                cmd.Parameters.AddWithValue("@Age", proprietaire.Age);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new Exception("User not found.");
                }
            }
        }

        public bool Delete(int id)
        {
            using (var conn = (MySqlConnection)_dbContext.CreateConnection()) // Use CreateConnection here
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM Proprietaire WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);

                int rowsAffected = cmd.ExecuteNonQuery();

                // Return true if a row was deleted, false otherwise
                return rowsAffected > 0;
            }
        }


        public User Login(UserLoginDTO user)
        {
            using (var conn = (MySqlConnection)_dbContext.CreateConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    "SELECT * FROM Proprietaire WHERE Email = @Email AND Password = @Password", conn);

                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            id = reader.GetInt32("Id"),
                            name = reader.GetString("Name"),
                            email = reader.GetString("Email"),
                            age = reader.GetInt32("Age")
                        };
                    }
                }
            }

            return null; // Return null if no match is found
        }

    }
}
