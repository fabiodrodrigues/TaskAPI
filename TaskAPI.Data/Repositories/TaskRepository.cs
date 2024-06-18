using Dapper;
using TaskAPI.Domain.Contracts.Repositories;
using TaskAPI.Domain.Entities;

namespace TaskAPI.Data.Repositories
{
    public class TaskRepository : BaseRepository, ITaskRepository
    {
        public TaskRepository(IConexaoBanco dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<IEnumerable<TaskItem>> Get()
        {
            using (var connection = GetDbConnection())
            {
                return await connection.QueryAsync<TaskItem>("SELECT * FROM Tasks ORDER BY Description ASC");
            }
        }

        public async Task<TaskItem> Get(Guid id)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<TaskItem>("SELECT * FROM Tasks WHERE Id = @Id", new { Id = id });
            }
        }

        public async Task Create(TaskItem task)
        {
            using (var connection = GetDbConnection())
            {
                var sql = "INSERT INTO Tasks (Id, Description, Completed) VALUES (@Id, @Description, @Completed)";
                await connection.ExecuteAsync(sql, task);
            }
        }

        public async Task Update(TaskItem task)
        {
            using (var connection = GetDbConnection())
            {
                var sql = "UPDATE Tasks SET Description = @Description, Completed = @Completed WHERE Id = @Id";
                await connection.ExecuteAsync(sql, task);
            }
        }

        public async Task Delete(Guid id)
        {
            using (var connection = GetDbConnection())
            {
                var sql = "DELETE FROM Tasks WHERE Id = @Id";
                await connection.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}
