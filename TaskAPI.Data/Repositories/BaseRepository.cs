using System.Data;
using TaskAPI.Domain.Contracts.Repositories;

namespace TaskAPI.Data.Repositories
{
    public class BaseRepository
    {
        private readonly IConexaoBanco _conexaoBanco;

        public BaseRepository(IConexaoBanco conexaoBanco)
        {
            _conexaoBanco = conexaoBanco;
        }

        public IDbConnection GetDbConnection()
        {
            return _conexaoBanco.GetConnection();
        }
    }
}
