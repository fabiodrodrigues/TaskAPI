using TaskAPI.Domain.Entities;

namespace TaskAPI.Domain.Contracts.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> Get();
        Task<TaskItem> Get(Guid id);
        Task Create(TaskItem task);
        Task Update(TaskItem task);
        Task Delete(Guid id);
    }
}
