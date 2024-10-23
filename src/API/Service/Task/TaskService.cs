using AutoMapper;
using System.Security.Claims;
using task_management.src.API.Base.Pagination;
using task_management.src.API.Interface;
using task_management.src.API.Interface.Task;
using task_management.src.API.Model;
using task_management.src.API.View.Task;

namespace task_management.src.API.Service.Task
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IMapper _mapper;

        private readonly int _userID;

        public TaskService(ITaskRepository taskRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _repository = taskRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _userID = GetUserIdFromToken();
        }

        public async Task<TaskItem> AddAsync(TaskItem task)
        {
            task.UserId = _userID;

            return await _repository.AddAsync(task);
        }

        public async Task<bool> DeleteAsync(TaskItem task)
        {
            return await _repository.DeleteAsync(task);
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            return await _repository.DeleteByIdAsync(id);
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<PagedResult<TaskItemResponse>> GetPagedTasksByUserIdAsync(int pageNumber, int pageSize)
        {

            (IEnumerable<TaskItem> tasks, int totalItems) = await _repository.GetPagedTasksByUserIdAsync(_userID, pageNumber, pageSize);

            var taskResponse = _mapper.Map<IEnumerable<TaskItemResponse>>(tasks);

            return new PagedResult<TaskItemResponse>
            {
                Items = taskResponse,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId)
        {
            return _repository.GetTasksByUserIdAsync(userId);
        }

        public async Task<TaskItem> UpdateAsync(TaskItem task)
        {
            task.UserId = _userID;

            return await _repository.UpdateAsync(task);
        }

        private int GetUserIdFromToken()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            return int.Parse(userId);
        }
    }
}
