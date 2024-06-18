using System.Text;
using TaskAPI.Domain.Contracts.Repositories;
using TaskAPI.Domain.Contracts.Services;
using TaskAPI.Domain.Entities;

namespace TaskAPI.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IEnumerable<TaskItem>> Get()
        {
            return await _taskRepository.Get();
        }

        public async Task<TaskItem> Get(Guid id)
        {
            ValidateId(id);

            return await _taskRepository.Get(id);
        }

        public async Task Create(TaskItem task)
        {
            ValidateTaskNotNull(task);
            ValidateTask(task);

            await _taskRepository.Create(task);
        }

        public async Task Update(TaskItem task)
        {
            ValidateTaskNotNull(task);
            ValidateTask(task);

            await _taskRepository.Update(task);
        }

        public async Task<bool> Delete(Guid id)
        {
            ValidateId(id);

            var task = await _taskRepository.Get(id);

            if (task == null)
            {
                return false;
            }

            await _taskRepository.Delete(id);
            return true;
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("O 'id' fornecido é inválido.");
            }
        }

        private static void ValidateTaskNotNull(TaskItem task)
        {
            if (task == null)
            {
                throw new ArgumentException("A tarefa não pode ser nula.");
            }
        }

        private static void ValidateTask(TaskItem task)
        {
            var errors = new StringBuilder();

            if (task.Id == Guid.Empty)
            {
                errors.AppendLine("O 'id' fornecido é inválido.");
            }

            if (string.IsNullOrWhiteSpace(task.Description))
            {
                errors.AppendLine("Descrição é um campo obrigatório.");
            }

            if (errors.Length > 0)
            {
                throw new ArgumentException(errors.ToString().Trim());
            }
        }
    }
}
