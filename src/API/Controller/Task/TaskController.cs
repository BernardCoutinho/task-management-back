using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using task_management.src.API.Interface.Task;
using task_management.src.API.Model;
using task_management.src.API.Service.Task;
using task_management.src.API.View.Task;

namespace task_management.src.API.Controller.Task
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _service;
        private readonly IMapper _mapper;

        public TaskController(ITaskService taskService, IMapper mapper)
        {
            _service = taskService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTaskAsync([FromBody] TaskItemRequest taskRequest)
        {
            if (taskRequest is null)
            {
                return BadRequest("Task data is null.");
            }
            try
            {
                TaskItem task = _mapper.Map<TaskItem>(taskRequest);

                await _service.AddAsync(task);

                TaskItemResponse response = _mapper.Map<TaskItemResponse>(task);
                return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, response);
            }
            catch (AutoMapperMappingException ex)
            {
                return StatusCode(500, "Mapping configuration issue.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var task = await _service.GetByIdAsync(id);
                if (task == null)
                {
                    return NotFound();
                }

                TaskItemResponse response = _mapper.Map<TaskItemResponse>(task);

                return Ok(response);
            }
            catch (AutoMapperMappingException ex)
            {
                return StatusCode(500, "Mapping configuration issue.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetTasksByUserId(int id)
        {
            try
            {
                var tasks = await _service.GetTasksByUserIdAsync(id);
                if (tasks == null)
                {
                    return NotFound();
                }

                TaskItemResponse response = _mapper.Map<TaskItemResponse>(tasks);

                return Ok(response);
            }
            catch (AutoMapperMappingException ex)
            {
                return StatusCode(500, "Mapping configuration issue.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTask([FromQuery] int pageNumber, int pageSize )
        {

            try
            {
                var response = await _service.GetPagedTasksByUserIdAsync(pageNumber, pageSize);


                return Ok(response);
            }
            catch (AutoMapperMappingException ex)
            {
                return StatusCode(500, "Mapping configuration issue.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTaskAsync([FromBody] TaskItemRequest taskRequest)
        {
            if (taskRequest is null)
            {
                return BadRequest("Task data is null.");
            }
            try
            {
                TaskItem task = _mapper.Map<TaskItem>(taskRequest);

                await _service.UpdateAsync(task);

                TaskItemResponse response = _mapper.Map<TaskItemResponse>(task);
                return Ok(response);
            }
            catch (AutoMapperMappingException ex)
            {
                return StatusCode(500, "Mapping configuration issue.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTaskAsync([FromBody] TaskItemRequest taskRequest)
        {
            if (taskRequest is null)
            {
                return BadRequest("Task data is null.");
            }
            try
            {
                TaskItem task = _mapper.Map<TaskItem>(taskRequest);

                await _service.DeleteAsync(task);

                TaskItemResponse response = _mapper.Map<TaskItemResponse>(task);
                return NoContent();
            }
            catch (AutoMapperMappingException ex)
            {
                return StatusCode(500, "Mapping configuration issue.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskByIdAsync(int id)
        {
            if (await _service.GetByIdAsync(id) is null)
            {
                return NotFound("Task data not exists.");
            }

            try
            {

                await _service.DeleteByIdAsync(id);

                return NoContent();
            }
            catch (AutoMapperMappingException ex)
            {
                return StatusCode(500, "Mapping configuration issue.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }
    }
}
