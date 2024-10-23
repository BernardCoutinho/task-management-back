using Moq;
using AutoMapper;
using task_management.src.API.Interface;
using task_management.src.API.Model;
using task_management.src.API.View.User;
using TaskManagement.src.API.Controller;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace task_management.tests.TaskManagement.UnitTests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            // Inicializa os mocks
            _userServiceMock = new Mock<IUserService>();
            _mapperMock = new Mock<IMapper>();

            // Passa o mock do IMapper no construtor do controlador
            _userController = new UserController(_userServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateUser_ReturnsCreated_WhenUserIsCreatedSuccessfully()
        {
            // Arrange
            var userRequest = new UserRequest { Username = "testUser", Email = "test@example.com", Password = "Test@123" };
            var newUser = new User { Id = 1, Username = "testUser", Email = "test@example.com" };
            var userResponse = new UserResponse { Id = 1, Username = "testUser", Email = "test@example.com" };

            _userServiceMock.Setup(s => s.CreateUserAsync(It.IsAny<UserRequest>()))
                            .ReturnsAsync(newUser);

            _mapperMock.Setup(m => m.Map<UserResponse>(It.IsAny<User>()))
                       .Returns(userResponse);

            // Act
            var result = await _userController.CreateUser(userRequest);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(userResponse, createdResult.Value);
        }

        [Fact]
        public async Task GetUserById_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testUser", Email = "test@example.com" };
            var userResponse = new UserResponse { Id = 1, Username = "testUser", Email = "test@example.com" };

            _userServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                            .ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<UserResponse>(It.IsAny<User>()))
                       .Returns(userResponse);

            // Act
            var result = await _userController.GetUserById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(userResponse, okResult.Value);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _userServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                            .ReturnsAsync((User)null);

            // Act
            var result = await _userController.GetUserById(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk_WhenUsersExist()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "testUser1", Email = "test1@example.com" },
                new User { Id = 2, Username = "testUser2", Email = "test2@example.com" }
            };

            var userResponses = new List<UserResponse>
            {
                new UserResponse { Id = 1, Username = "testUser1", Email = "test1@example.com" },
                new UserResponse { Id = 2, Username = "testUser2", Email = "test2@example.com" }
            };

            _userServiceMock.Setup(s => s.GetAllAsync())
                            .ReturnsAsync(users);

            _mapperMock.Setup(m => m.Map<IEnumerable<UserResponse>>(It.IsAny<IEnumerable<User>>()))
                       .Returns(userResponses);

            // Act
            var result = await _userController.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(userResponses, okResult.Value);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOk_WhenUserIsUpdatedSuccessfully()
        {
            // Arrange
            var userRequest = new UserRequest { Username = "updatedUser", Email = "updated@example.com", Password = "Test@123" };
            var existingUser = new User { Id = 1, Username = "testUser", Email = "test@example.com" };
            var updatedUser = new User { Id = 1, Username = "updatedUser", Email = "updated@example.com" };
            var userResponse = new UserResponse { Id = 1, Username = "updatedUser", Email = "updated@example.com" };

            _userServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                            .ReturnsAsync(existingUser);

            _userServiceMock.Setup(s => s.UpdateAsync(It.IsAny<User>()))
                            .ReturnsAsync(updatedUser);

            _mapperMock.Setup(m => m.Map(It.IsAny<UserRequest>(), It.IsAny<User>()))
                       .Returns(updatedUser);

            _mapperMock.Setup(m => m.Map<UserResponse>(It.IsAny<User>()))
                       .Returns(userResponse);

            // Act
            var result = await _userController.UpdateUser(1, userRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(userResponse, okResult.Value);
        }

        [Fact]
        public async Task DeleteUserById_ReturnsNoContent_WhenUserIsDeleted()
        {
            // Arrange
            var existingUser = new User { Id = 1, Username = "testUser", Email = "test@example.com" };

            _userServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                            .ReturnsAsync(existingUser);

            _userServiceMock.Setup(s => s.DeleteByIdAsync(It.IsAny<int>()))
                            .ReturnsAsync(true);

            // Act
            var result = await _userController.DeleteUserById(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUserById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _userServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                            .ReturnsAsync((User)null);

            // Act
            var result = await _userController.DeleteUserById(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent_WhenUserIsDeletedByUsernameOrEmail()
        {
            // Arrange
            var userRequest = new UserRequest { Username = "testUser", Email = "test@example.com" };
            var existingUser = new User { Id = 1, Username = "testUser", Email = "test@example.com" };

            _userServiceMock.Setup(s => s.GetByUsernameOrEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                            .ReturnsAsync(existingUser);

            _userServiceMock.Setup(s => s.DeleteAsync(It.IsAny<User>()))
                            .ReturnsAsync(true);

            // Act
            var result = await _userController.DeleteUser(userRequest);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExistByUsernameOrEmail()
        {
            // Arrange
            var userRequest = new UserRequest { Username = "testUser", Email = "test@example.com" };

            _userServiceMock.Setup(s => s.GetByUsernameOrEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                            .ReturnsAsync((User)null);

            // Act
            var result = await _userController.DeleteUser(userRequest);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
