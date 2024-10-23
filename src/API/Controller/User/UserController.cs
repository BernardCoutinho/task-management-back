using task_management.src.API.Interface;
using task_management.src.API.View.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using task_management.src.API.Model;

namespace TaskManagement.src.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _service = userService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserRequest request)
        {
            if (request == null)
            {
                return BadRequest("User data is null.");
            }

            try
            {
                User user = await _service.CreateUserAsync(request);
                UserResponse response = _mapper.Map<UserResponse>(user);

                return CreatedAtAction(nameof(GetUserById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _service.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var response = _mapper.Map<UserResponse>(user);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _service.GetAllAsync();
                var response = _mapper.Map<IEnumerable<UserResponse>>(users);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserRequest request)
        {
            if (request == null)
            {
                return BadRequest("User data is null.");
            }

            try
            {
                var user = await _service.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var updatedUser = _mapper.Map(request, user);
                await _service.UpdateAsync(updatedUser);

                var response = _mapper.Map<UserResponse>(updatedUser);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserById(int id)
        {
            try
            {
                var user = await _service.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                await _service.DeleteByIdAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

      
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromBody] UserRequest request)
        {
            if (request == null)
            {
                return BadRequest("User data is null.");
            }

            try
            {
                var user = await _service.GetByUsernameOrEmailAsync(request.Username, request.Email);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                await _service.DeleteAsync(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
