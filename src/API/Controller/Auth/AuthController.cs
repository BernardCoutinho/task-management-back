
namespace task_management.src.API.Controller.Auth
{
    using Microsoft.AspNetCore.Mvc;
    using task_management.src.API.Interface.Login;
    using task_management.src.API.View.Login;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public AuthController(ILoginService authService)
        {
            _loginService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _loginService.Authenticate(request.Username, request.Password);

            if (token == null)
                return Unauthorized(new { message = "Credenciais inválidas" });

            return Ok(new { Token = "Bearer " + token });
        }
    }
}
