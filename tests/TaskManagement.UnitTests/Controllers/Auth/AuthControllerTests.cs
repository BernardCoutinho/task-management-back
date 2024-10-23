namespace task_management.tests.TaskManagement.UnitTests.Controllers.Auth
{
    using Moq;
    using task_management.src.API.Controller.Auth;
    using task_management.src.API.Interface.Login;
    using task_management.src.API.View.Login;
    using Microsoft.AspNetCore.Mvc;
    using Xunit;
    using System.Threading.Tasks;

    
    public class AuthControllerTests
    {
        private readonly AuthController _controller;
        private readonly Mock<ILoginService> _loginServiceMock;

        public AuthControllerTests()
        {
            _loginServiceMock = new Mock<ILoginService>();
            _controller = new AuthController(_loginServiceMock.Object);
        }

        [Fact]
        public async Task Login_ReturnsOkResult_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "user", Password = "password" };
            _loginServiceMock.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                                .ReturnsAsync("valid_token");

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "user", Password = "wrong_password" };
            _loginServiceMock.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                                .ReturnsAsync((string)null);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Username", "Required");
            var loginRequest = new LoginRequest { Username = "", Password = "password" };

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
    }
    
}
