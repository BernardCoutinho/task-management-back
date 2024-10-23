using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using task_management.src.API.Controller.Task;
using task_management.src.API.Interface.Task;
using task_management.src.API.Model;
using task_management.src.API.View.Task;
using Xunit;

namespace task_management.tests.TaskManagement.UnitTests.Controllers
{
    public class TaskControllerTests
    {
        private readonly Mock<ITaskService> _taskServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TaskController _taskController;

        public TaskControllerTests()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _mapperMock = new Mock<IMapper>();
            _taskController = new TaskController(_taskServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateTaskAsync_ReturnsCreatedResult_WhenTaskIsCreated()
        {
            // Arrange
            var taskRequest = new TaskItemRequest { Title = "New Task", Description = "Description of the task" };
            var taskItem = new TaskItem { Id = 1, Title = "New Task", Description = "Description of the task" };
            var taskResponse = new TaskItemResponse { Id = 1, Title = "New Task", Description = "Description of the task" };

            // Simula o mapeamento de TaskItemRequest para TaskItem
            _mapperMock.Setup(m => m.Map<TaskItem>(It.IsAny<TaskItemRequest>())).Returns(taskItem);

            // Simula o comportamento do serviço ao adicionar uma nova task
            _taskServiceMock.Setup(s => s.AddAsync(It.IsAny<TaskItem>())).ReturnsAsync(taskItem);

            // Simula o mapeamento de TaskItem para TaskItemResponse
            _mapperMock.Setup(m => m.Map<TaskItemResponse>(It.IsAny<TaskItem>())).Returns(taskResponse);

            // Act
            var result = await _taskController.CreateTaskAsync(taskRequest);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(taskResponse, createdResult.Value);
        }

        [Fact]
        public async Task GetTaskById_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            _taskServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((TaskItem)null);

            // Act
            var result = await _taskController.GetTaskById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetTaskById_ReturnsOkResult_WhenTaskExists()
        {
            // Arrange
            var taskItem = new TaskItem { Id = 1, Title = "Existing Task", Description = "Description of the task" };
            var taskResponse = new TaskItemResponse { Id = 1, Title = "Existing Task", Description = "Description of the task" };

            _taskServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(taskItem);
            _mapperMock.Setup(m => m.Map<TaskItemResponse>(taskItem)).Returns(taskResponse);

            // Act
            var result = await _taskController.GetTaskById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(taskResponse, okResult.Value);
        }

        [Fact]
        public async Task DeleteTaskByIdAsync_ReturnsNoContent_WhenTaskIsDeleted()
        {
            // Arrange
            // Arrange: Simula que a tarefa existe
            _taskServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new TaskItem { Id = 1, Title = "Test Task" });

            // Simula que a tarefa foi deletada com sucesso
            _taskServiceMock.Setup(s => s.DeleteByIdAsync(It.IsAny<int>())).ReturnsAsync(true);

            // Act: Chama o método do controlador
            var result = await _taskController.DeleteTaskByIdAsync(1);

            // Assert: Verifica se o retorno foi 204 No Content
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTaskByIdAsync_ReturnsNotFound_WhenTaskIsNotFound()
        {
            // Arrange
            _taskServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((TaskItem)null);

            // Act
            var result = await _taskController.DeleteTaskByIdAsync(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
