
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace Laverie.API.Infrastructure.context
{
    public class AppDbContext
    {
        private readonly string _connectionString;

        public AppDbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MySQLConnection")
                                ?? throw new InvalidOperationException("Connection string 'MySQLConnection' is not configured.");
        }

        public IDbConnection CreateConnection()
        {
            var connection = new MySqlConnection(_connectionString);
            return connection;
        }
    }
}