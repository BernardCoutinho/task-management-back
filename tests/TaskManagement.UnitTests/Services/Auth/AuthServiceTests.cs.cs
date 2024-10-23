namespace task_management.tests.TaskManagement.UnitTests.Services.Auth
{
    using Moq;
    using task_management.src.API.Interface;
    using task_management.src.API.Interface.Login;
    using task_management.src.API.Model;
    using task_management.src.API.Service.Auth;
    using Microsoft.Extensions.Configuration;
    using Microsoft.AspNetCore.Identity;
    using Xunit;
    using System.Threading.Tasks;

    public class AuthServiceTests
    {
        private readonly AuthService _authService;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();

            _authService = new AuthService(_configurationMock.Object, _userRepositoryMock.Object, _passwordHasherMock.Object);
        }

        [Fact]
        public async Task Authenticate_ReturnsNull_WhenUserNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetByUsernameOrEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.Authenticate("user", "password");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Authenticate_ReturnsNull_WhenPasswordIsIncorrect()
        {
            // Arrange
            var user = new User { Username = "user", PasswordHash = "hashed_password" };
            _userRepositoryMock.Setup(repo => repo.GetByUsernameOrEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                                .ReturnsAsync(user);
            _passwordHasherMock.Setup(hasher => hasher.VerifyHashedPassword(user, "hashed_password", "wrong_password"))
                                .Returns(PasswordVerificationResult.Failed);

            // Act
            var result = await _authService.Authenticate("user", "wrong_password");

            // Assert
            Assert.Null(result);
        }

        //[Fact]
        //public async Task Authenticate_ReturnsToken_WhenCredentialsAreValid()
        //{
        //    // Arrange
        //    var user = new User { Username = "user", PasswordHash = "hashed_password" };
        //    _userRepositoryMock.Setup(repo => repo.GetByUsernameOrEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
        //                        .ReturnsAsync(user);

        //    _passwordHasherMock.Setup(hasher => hasher.VerifyHashedPassword(user, "hashed_password", "password"))
        //                        .Returns(PasswordVerificationResult.Success);

        //    _configurationMock.SetupGet(x => x["Jwt:Key"]).Returns("my_secret_key");
        //    _configurationMock.SetupGet(x => x["Jwt:Issuer"]).Returns("issuer");
        //    _configurationMock.SetupGet(x => x["Jwt:Audience"]).Returns("audience");

        //    // Act
        //    var result = await _authService.Authenticate("user", "password");

        //    // Assert
        //    Assert.NotNull(result);
        //}
    }
    

}
