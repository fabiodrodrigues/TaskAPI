using TaskAPI.Domain.Entities;

namespace TaskAPI.Domain.Contracts.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> Get();
        Task<TaskItem> Get(Guid id);
        Task Create(TaskItem task);
        Task Update(TaskItem task);
        Task<bool> Delete(Guid id);
    }
}
