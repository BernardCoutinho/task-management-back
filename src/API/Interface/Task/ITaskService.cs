using task_management.src.API.Base;
using task_management.src.API.Base.Pagination;
using task_management.src.API.Model;
using task_management.src.API.View.Task;

namespace task_management.src.API.Interface.Task
{
    public interface ITaskService : IService<TaskItem, int>
    {
        Task<PagedResult<TaskItemResponse>> GetPagedTasksByUserIdAsync(int pageNumber, int pageSize);

        Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId);
    }
}
