using Bogus;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using TaskAPI.API.Controllers;
using TaskAPI.Domain.Contracts.Services;
using TaskAPI.Domain.Entities;

namespace TaskAPI.Test.Controllers
{
    public class TasksControllerTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly TasksController _tasksController;

        public TasksControllerTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _tasksController = new TasksController(_mockTaskService.Object);
        }

        [Fact]
        public async Task Should_Get_ReturnsOkResult_WithListOfTasks()
        {
            var tasks = new Faker<TaskItem>()
                .RuleFor(t => t.Id, f => f.Random.Guid())
                .RuleFor(t => t.Description, f => f.Random.Words(5))
                .RuleFor(t => t.Completed, f => f.Random.Bool())
                .Generate(2);

            _mockTaskService.Setup(service => service.Get()).ReturnsAsync(tasks);

            var result = (await _tasksController.Get()).Result as OkObjectResult;

            result.Value.Should().BeEquivalentTo(tasks);
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task Should_Get_ReturnsNotFoundResult_WhenNoTasksFound()
        {
            _mockTaskService.Setup(service => service.Get()).ReturnsAsync((IEnumerable<TaskItem>)null);

            var result = await _tasksController.Get();

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Should_Get_ReturnsNotFoundResult_WhenTasksListIsEmpty()
        {
            var tasks = new List<TaskItem>();
            _mockTaskService.Setup(service => service.Get()).ReturnsAsync(tasks);

            var result = await _tasksController.Get();

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Should_Get_ReturnsInternalServerError_WhenServiceThrowsException()
        {
            var message = "Simulated exception";
            _mockTaskService.Setup(service => service.Get()).ThrowsAsync(new Exception(message));

            var result = await _tasksController.Get();

            var objectResult = result.Result as ObjectResult;
            objectResult.Value.Should().BeEquivalentTo(new { Message = message });
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Should_Get_ReturnsOk_WhenTaskIsFound()
        {
            var taskId = Guid.NewGuid();
            var taskItem = new Faker<TaskItem>()
                .RuleFor(t => t.Id, f => taskId)
                .RuleFor(t => t.Description, f => f.Random.Words(5))
                .RuleFor(t => t.Completed, f => f.Random.Bool())
                .Generate();
            _mockTaskService.Setup(service => service.Get(taskId)).ReturnsAsync(taskItem);

            var result = await _tasksController.Get(taskId);

            var okResult = result.Result as OkObjectResult;
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value.Should().BeEquivalentTo(taskItem);
        }

        [Fact]
        public async Task Should_Get_ReturnsNotFound_WhenTaskIsNotFound()
        {
            var taskId = Guid.NewGuid();
            _mockTaskService.Setup(service => service.Get(taskId)).ReturnsAsync((TaskItem)null);

            var result = await _tasksController.Get(taskId);

            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_Get_ReturnsBadRequest_WhenArgumentExceptionIsThrown()
        {
            var taskId = Guid.NewGuid();
            var errorMessage = "Invalid argument";
            _mockTaskService.Setup(service => service.Get(taskId)).ThrowsAsync(new ArgumentException(errorMessage));

            var result = await _tasksController.Get(taskId);

            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            badRequestResult.Value.Should().BeEquivalentTo(new { Message = errorMessage });
        }

        [Fact]
        public async Task Should_Get_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var taskId = Guid.NewGuid();
            var errorMessage = "Simulated exception";
            _mockTaskService.Setup(service => service.Get(taskId)).ThrowsAsync(new Exception(errorMessage));

            var result = await _tasksController.Get(taskId);

            var internalServerErrorResult = result.Result as ObjectResult;
            internalServerErrorResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            internalServerErrorResult.Value.Should().BeEquivalentTo(new { Message = errorMessage });
        }

        [Fact]
        public async Task Should_Create_ReturnsCreatedAtAction_WhenTaskIsCreated()
        {
            var taskItem = new Faker<TaskItem>()
                .RuleFor(t => t.Id, f => Guid.NewGuid())
                .RuleFor(t => t.Description, f => f.Random.Words(5))
                .RuleFor(t => t.Completed, f => f.Random.Bool())
                .Generate();
            _mockTaskService.Setup(service => service.Create(taskItem)).Returns(Task.CompletedTask);

            var result = await _tasksController.Create(taskItem);

            var createdAtActionResult = result as CreatedAtActionResult;
            createdAtActionResult.Should().NotBeNull();
            createdAtActionResult.StatusCode.Should().Be((int)HttpStatusCode.Created);
            createdAtActionResult.Value.Should().BeEquivalentTo(taskItem);
        }

        [Fact]
        public async Task Should_Create_ReturnsBadRequest_WhenArgumentExceptionIsThrown()
        {
            var taskItem = new Faker<TaskItem>()
                .RuleFor(t => t.Id, f => Guid.NewGuid())
                .RuleFor(t => t.Description, f => f.Random.Words(5))
                .RuleFor(t => t.Completed, f => f.Random.Bool())
                .Generate();
            var errorMessage = "Invalid argument";
            _mockTaskService.Setup(service => service.Create(taskItem)).ThrowsAsync(new ArgumentException(errorMessage));

            var result = await _tasksController.Create(taskItem);

            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            badRequestResult.Value.Should().BeEquivalentTo(new { Message = errorMessage });
        }

        [Fact]
        public async Task Should_Create_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var taskItem = new Faker<TaskItem>()
                .RuleFor(t => t.Id, f => Guid.NewGuid())
                .RuleFor(t => t.Description, f => f.Random.Words(5))
                .RuleFor(t => t.Completed, f => f.Random.Bool())
                .Generate();
            var errorMessage = "Simulated exception";
            _mockTaskService.Setup(service => service.Create(taskItem)).ThrowsAsync(new Exception(errorMessage));

            var result = await _tasksController.Create(taskItem);

            var internalServerErrorResult = result as ObjectResult;
            internalServerErrorResult.Should().NotBeNull();
            internalServerErrorResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            internalServerErrorResult.Value.Should().BeEquivalentTo(new { Message = errorMessage });
        }

        [Fact]
        public async Task Should_Update_ReturnsBadRequest_WhenIdMismatch()
        {
            var taskId = Guid.NewGuid();
            var taskItem = new Faker<TaskItem>()
                .RuleFor(t => t.Id, f => Guid.NewGuid())
                .RuleFor(t => t.Description, f => f.Random.Words(5))
                .RuleFor(t => t.Completed, f => f.Random.Bool())
                .Generate();

            var result = await _tasksController.Update(taskId, taskItem);

            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            badRequestResult.Value.Should().BeEquivalentTo(new { Message = "O ID da URL não corresponde ao ID da tarefa." });
        }

        [Fact]
        public async Task Should_Update_ReturnsNotFound_WhenTaskNotFound()
        {
            var taskId = Guid.NewGuid();
            var taskItem = new Faker<TaskItem>()
                .RuleFor(t => t.Id, f => taskId)
                .RuleFor(t => t.Description, f => f.Random.Words(5))
                .RuleFor(t => t.Completed, f => f.Random.Bool())
                .Generate();

            _mockTaskService.Setup(service => service.Get(taskId)).ReturnsAsync((TaskItem)null);

            var result = await _tasksController.Update(taskId, taskItem);

            var notFoundResult = result as NotFoundResult;
            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_Update_ReturnsNoContent_WhenTaskIsUpdated()
        {
            var taskId = Guid.NewGuid();
            var taskItem = new Faker<TaskItem>()
                .RuleFor(t => t.Id, f => taskId)
                .RuleFor(t => t.Description, f => f.Random.Words(5))
                .RuleFor(t => t.Completed, f => f.Random.Bool())
                .Generate();

            _mockTaskService.Setup(service => service.Get(taskId)).ReturnsAsync(taskItem);
            _mockTaskService.Setup(service => service.Update(taskItem)).Returns(Task.CompletedTask);

            var result = await _tasksController.Update(taskId, taskItem);

            var noContentResult = result as NoContentResult;
            noContentResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Should_Update_ReturnsBadRequest_WhenArgumentExceptionIsThrown()
        {
            var taskId = Guid.NewGuid();
            var taskItem = new Faker<TaskItem>()
                .RuleFor(t => t.Id, f => taskId)
                .RuleFor(t => t.Description, f => f.Random.Words(5))
                .RuleFor(t => t.Completed, f => f.Random.Bool())
                .Generate();
            var errorMessage = "Invalid argument";

            _mockTaskService.Setup(service => service.Get(taskId)).ReturnsAsync(taskItem);
            _mockTaskService.Setup(service => service.Update(taskItem)).ThrowsAsync(new ArgumentException(errorMessage));

            var result = await _tasksController.Update(taskId, taskItem);

            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            badRequestResult.Value.Should().BeEquivalentTo(new { Message = errorMessage });
        }

        [Fact]
        public async Task Should_Update_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var taskId = Guid.NewGuid();
            var taskItem = new Faker<TaskItem>()
                .RuleFor(t => t.Id, f => taskId)
                .RuleFor(t => t.Description, f => f.Random.Words(5))
                .RuleFor(t => t.Completed, f => f.Random.Bool())
                .Generate();
            var errorMessage = "Simulated exception";

            _mockTaskService.Setup(service => service.Get(taskId)).ReturnsAsync(taskItem);
            _mockTaskService.Setup(service => service.Update(taskItem)).ThrowsAsync(new Exception(errorMessage));

            var result = await _tasksController.Update(taskId, taskItem);

            var internalServerErrorResult = result as ObjectResult;
            internalServerErrorResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            internalServerErrorResult.Value.Should().BeEquivalentTo(new { Message = errorMessage });
        }

        [Fact]
        public async Task Should_Delete_ReturnsNotFound_WhenTaskNotFound()
        {
            var taskId = Guid.NewGuid();
            _mockTaskService.Setup(service => service.Delete(taskId)).ReturnsAsync(false);

            var result = await _tasksController.Delete(taskId);

            var notFoundResult = result as NotFoundResult;
            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_Delete_ReturnsNoContent_WhenTaskIsDeleted()
        {
            var taskId = Guid.NewGuid();
            _mockTaskService.Setup(service => service.Delete(taskId)).ReturnsAsync(true);


            var result = await _tasksController.Delete(taskId);

            var noContentResult = result as NoContentResult;
            noContentResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Should_Delete_ReturnsBadRequest_WhenArgumentExceptionIsThrown()
        {
            var taskId = Guid.NewGuid();
            var errorMessage = "Invalid argument";
            _mockTaskService.Setup(service => service.Delete(taskId)).ThrowsAsync(new ArgumentException(errorMessage));

            var result = await _tasksController.Delete(taskId);

            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            badRequestResult.Value.Should().BeEquivalentTo(new { Message = errorMessage });
        }

        [Fact]
        public async Task Should_Delete_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var taskId = Guid.NewGuid();
            var errorMessage = "Simulated exception";
            _mockTaskService.Setup(service => service.Delete(taskId)).ThrowsAsync(new Exception(errorMessage));

            var result = await _tasksController.Delete(taskId);

            var internalServerErrorResult = result as ObjectResult;
            internalServerErrorResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            internalServerErrorResult.Value.Should().BeEquivalentTo(new { Message = errorMessage });
        }
    }
}
