using Bogus;
using Moq;
using TaskAPI.Application.Services;
using TaskAPI.Domain.Contracts.Repositories;
using TaskAPI.Domain.Entities;

namespace TaskAPI.Test.Services
{
    public class TaskServiceTests
    {
        private readonly TaskService _taskService;
        private readonly Mock<ITaskRepository> _taskRepositoryMock;

        public TaskServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _taskService = new TaskService(_taskRepositoryMock.Object);
        }

        [Fact]
        public async Task Should_Get_ShouldReturnTasks()
        {
            var tasks = new Faker<TaskItem>()
                .RuleFor(t => t.Id, f => f.Random.Guid())
                .RuleFor(t => t.Description, f => f.Random.Words(5))
                .RuleFor(t => t.Completed, f => f.Random.Bool())
                .Generate(3);
            _taskRepositoryMock.Setup(repo => repo.Get()).ReturnsAsync(tasks);

            var result = await _taskService.Get();

            result.Should().BeEquivalentTo(tasks);
        }

        [Fact]
        public async Task Should_Get_ById_ShouldReturnTask()
        {
            var taskId = Guid.NewGuid();
            var taskItem = new Faker<TaskItem>()
                .RuleFor(t => t.Id, f => taskId)
                .RuleFor(t => t.Description, f => f.Random.Words(5))
                .RuleFor(t => t.Completed, f => f.Random.Bool())
                .Generate();
            _taskRepositoryMock.Setup(repo => repo.Get(taskId)).ReturnsAsync(taskItem);

            var result = await _taskService.Get(taskId);

            result.Should().BeEquivalentTo(taskItem);
        }

        [Fact]
        public void Should_Get_ById_ShouldThrowArgumentException_WhenIdIsInvalid()
        {
            var invalidId = Guid.Empty;

            Func<Task> act = async () => await _taskService.Get(invalidId);

            act.Should().ThrowAsync<ArgumentException>().WithMessage("O 'id' fornecido é inválido.");
        }

        [Fact]
        public async Task Should_Create_ShouldCallRepository_WhenTaskIsValid()
        {
            var task = new Faker<TaskItem>()
                .RuleFor(t => t.Id, f => f.Random.Guid())
                .RuleFor(t => t.Description, f => f.Random.Words(5))
                .RuleFor(t => t.Completed, f => f.Random.Bool())
                .Generate();

            await _taskService.Create(task);

            _taskRepositoryMock.Verify(repo => repo.Create(task), Times.Once);
        }

        [Fact]
        public void Should_Create_ShouldThrowArgumentException_WhenTaskIsNull()
        {
            Func<Task> act = async () => await _taskService.Create(null);

            act.Should().ThrowAsync<ArgumentException>().WithMessage("A tarefa não pode ser nula.");
        }

        [Fact]
        public void Should_Create_ShouldThrowArgumentException_WhenTaskIsInvalid()
        {
            var task = new TaskItem { Id = Guid.Empty, Description = " " };

            Func<Task> act = async () => await _taskService.Create(task);

            act.Should().ThrowAsync<ArgumentException>().WithMessage("O 'id' fornecido é inválido.\nDescrição é um campo obrigatório.");
        }

        [Fact]
        public async Task Should_Update_ShouldCallRepository_WhenTaskIsValid()
        {
            var task = new Faker<TaskItem>()
                .RuleFor(t => t.Id, f => f.Random.Guid())
                .RuleFor(t => t.Description, f => f.Random.Words(5))
                .RuleFor(t => t.Completed, f => f.Random.Bool())
                .Generate();

            await _taskService.Update(task);

            _taskRepositoryMock.Verify(repo => repo.Update(task), Times.Once);
        }

        [Fact]
        public void Should_Update_ShouldThrowArgumentException_WhenTaskIsNull()
        {
            Func<Task> act = async () => await _taskService.Update(null);

            act.Should().ThrowAsync<ArgumentException>().WithMessage("A tarefa não pode ser nula.");
        }

        [Fact]
        public void Should_Update_ShouldThrowArgumentException_WhenTaskIsInvalid()
        {
            var task = new TaskItem { Id = Guid.Empty, Description = " " };

            Func<Task> act = async () => await _taskService.Update(task);

            act.Should().ThrowAsync<ArgumentException>().WithMessage("O 'id' fornecido é inválido.\nDescrição é um campo obrigatório.");
        }

        [Fact]
        public async Task Should_Delete_ShouldReturnTrue_WhenTaskExists()
        {
            var taskId = Guid.NewGuid();
            var task = new Faker<TaskItem>()
                .RuleFor(t => t.Id, f => taskId)
                .RuleFor(t => t.Description, f => f.Random.Words(5))
                .RuleFor(t => t.Completed, f => f.Random.Bool())
                .Generate();
            _taskRepositoryMock.Setup(repo => repo.Get(taskId)).ReturnsAsync(task);

            // Act
            var result = await _taskService.Delete(taskId);

            // Assert
            result.Should().BeTrue();
            _taskRepositoryMock.Verify(repo => repo.Delete(task.Id), Times.Once);
        }

        [Fact]
        public async Task Should_Delete_ShouldReturnFalse_WhenTaskDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _taskRepositoryMock.Setup(repo => repo.Get(id)).ReturnsAsync((TaskItem)null);

            // Act
            var result = await _taskService.Delete(id);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Should_Delete_ShouldThrowArgumentException_WhenIdIsInvalid()
        {
            var invalidId = Guid.Empty;

            Func<Task> act = async () => await _taskService.Delete(invalidId);

            act.Should().ThrowAsync<ArgumentException>().WithMessage("O 'id' fornecido é inválido.");
        }
    }
}