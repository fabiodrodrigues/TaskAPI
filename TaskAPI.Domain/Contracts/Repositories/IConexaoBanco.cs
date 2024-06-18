using Microsoft.Data.SqlClient;
using System.Data;

namespace TaskAPI.Domain.Contracts.Repositories
{
    public interface IConexaoBanco
    {
        IDbConnection GetConnection();
    }
}
