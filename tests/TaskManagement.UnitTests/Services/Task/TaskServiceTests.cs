using Moq;
using AutoMapper;
using task_management.src.API.Interface;
using task_management.src.API.Model;
using task_management.src.API.Service.Task;
using Xunit;
using System.Security.Claims;
using task_management.src.API.Config;


namespace task_management.tests.TaskManagement.UnitTests.Services
{
    public class TaskServiceTests
    {


       
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _mapperMock = new Mock<IMapper>();

            // Simular a extração do userId do token JWT
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, "1")
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext.User).Returns(userClaims);

            _taskService = new TaskService(_taskRepositoryMock.Object, _mapperMock.Object, _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddTask_WithUserIdFromToken()
        {
            // Arrange
            var taskItem = new TaskItem { Id = 1, Title = "New Task", UserId = 1 };
            _taskRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<TaskItem>()))
                                .ReturnsAsync(taskItem);

            // Act
            var result = await _taskService.AddAsync(taskItem);

            // Assert
            _taskRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<TaskItem>()), Times.Once);
            Assert.Equal(1, result.Id); // Verifica se o Id é retornado corretamente
            Assert.Equal(1, taskItem.UserId); // Verifica se o userId foi adicionado corretamente
        }

        [Fact]
        public async Task GetPagedTasksByUserIdAsync_ShouldReturnPagedResult()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "Task 1", Description = "Description Task 1", UserId = 1 },
                new TaskItem { Id = 2, Title = "Task 2", Description = "Description Task 2", UserId = 1 }
            };

            // Configuração do AutoMapper no teste
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile()); // Adiciona o perfil de mapeamento
            });
            var mapper = mockMapper.CreateMapper();

            // Simula o contexto HTTP e o token com o userId
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "1") // Simula o userId extraído do token
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext.User).Returns(userClaims);

            // Simula o retorno do repositório com duas tasks e total de 2
            _taskRepositoryMock.Setup(repo => repo.GetPagedTasksByUserIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                                .ReturnsAsync((tasks, 2));

            // Instancia o serviço com o mock do AutoMapper
            var taskService = new TaskService(_taskRepositoryMock.Object, mapper, _httpContextAccessorMock.Object);

            // Act
            var result = await taskService.GetPagedTasksByUserIdAsync(1, 2);

            // Assert
            Assert.NotNull(result);  // Verifica se o resultado não é nulo
            Assert.Equal(2, result.TotalItems); // Verifica o número total de itens
            Assert.Equal(2, result.Items.Count()); // Verifica se há 2 itens na lista retornada
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldReturnTrue_WhenTaskIsDeleted()
        {
            // Arrange
            _taskRepositoryMock.Setup(repo => repo.DeleteByIdAsync(It.IsAny<int>())).ReturnsAsync(true);

            // Act
            var result = await _taskService.DeleteByIdAsync(1);

            // Assert
            Assert.True(result);
        }
    }
    
}
