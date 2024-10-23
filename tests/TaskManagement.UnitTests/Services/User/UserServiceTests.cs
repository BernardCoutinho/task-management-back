using Moq;
using task_management.src.API.Interface;
using task_management.src.API.Model;
using task_management.src.API.Service;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace task_management.tests.TaskManagement.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
            _userService = new UserService(_userRepositoryMock.Object, _passwordHasherMock.Object);
        }

        // Teste para o método DeleteAsync
        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenUserIsDeletedSuccessfully()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testUser", Email = "test@example.com" };

            _userRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<User>()))
                               .ReturnsAsync(true); // Simula exclusão bem-sucedida

            // Act
            var result = await _userService.DeleteAsync(user);

            // Assert
            Assert.True(result);
        }

        // Teste para o método DeleteByIdAsync
        [Fact]
        public async Task DeleteByIdAsync_ReturnsTrue_WhenUserIsDeletedSuccessfully()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.DeleteByIdAsync(It.IsAny<int>()))
                               .ReturnsAsync(true); // Simula exclusão bem-sucedida

            // Act
            var result = await _userService.DeleteByIdAsync(1);

            // Assert
            Assert.True(result);
        }

        // Teste para o método GetAllAsync
        [Fact]
        public async Task GetAllAsync_ReturnsListOfUsers_WhenUsersExist()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "testUser1", Email = "test1@example.com" },
                new User { Id = 2, Username = "testUser2", Email = "test2@example.com" }
            };

            _userRepositoryMock.Setup(r => r.GetAllAsync())
                               .ReturnsAsync(users); // Simula a obtenção de uma lista de usuários

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        // Teste para o método GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testUser", Email = "test@example.com" };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                               .ReturnsAsync(user); // Simula que o usuário existe

            // Act
            var result = await _userService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        // Teste para o método GetByIdAsync quando o usuário não existe
        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                               .ReturnsAsync((User)null); // Simula que o usuário não existe

            // Act
            var result = await _userService.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        // Teste para o método UpdateAsync
        [Fact]
        public async Task UpdateAsync_UpdatesUser_WhenUserIsValid()
        {
            // Arrange
            var existingUser = new User { Id = 1, Username = "existingUser", Email = "existing@example.com" };
            var updatedUser = new User { Id = 1, Username = "updatedUser", Email = "updated@example.com", PasswordHash = "newpasswordhash" };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                               .ReturnsAsync(existingUser); // Simula que o usuário existe
            _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                               .ReturnsAsync(updatedUser); // Simula atualização bem-sucedida

            // Act
            var result = await _userService.UpdateAsync(updatedUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("updatedUser", result.Username);
            Assert.Equal("updated@example.com", result.Email);
        }

        // Teste para o método UpdateAsync quando o usuário não existe
        [Fact]
        public async Task UpdateAsync_ThrowsException_WhenUserDoesNotExist()
        {
            // Arrange
            var updatedUser = new User { Id = 1, Username = "updatedUser", Email = "updated@example.com" };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                               .ReturnsAsync((User)null); // Simula que o usuário não existe

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _userService.UpdateAsync(updatedUser));
            Assert.Equal("Usuário não encontrado.", exception.Message);
        }

        // Teste para o método GetByUsernameOrEmailAsync
        [Fact]
        public async Task GetByUsernameOrEmailAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Username = "testUser", Email = "test@example.com" };

            _userRepositoryMock.Setup(r => r.GetByUsernameOrEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                               .ReturnsAsync(user); // Simula que o usuário existe

            // Act
            var result = await _userService.GetByUsernameOrEmailAsync("testUser", "test@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testUser", result.Username);
        }

        // Teste para o método GetByUsernameOrEmailAsync quando o usuário não existe
        [Fact]
        public async Task GetByUsernameOrEmailAsync_ThrowsException_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetByUsernameOrEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                               .ReturnsAsync((User)null); // Simula que o usuário não existe

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _userService.GetByUsernameOrEmailAsync("testUser", "test@example.com"));
            Assert.Equal("Usuário não encontrado.", exception.Message);
        }
    }
}
