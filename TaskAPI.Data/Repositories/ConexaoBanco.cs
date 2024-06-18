using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using TaskAPI.Domain.Contracts.Repositories;

namespace TaskAPI.Data.Repositories
{
    public class ConexaoBanco : IConexaoBanco
    {
        private readonly string _connectionString;

        public ConexaoBanco(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
