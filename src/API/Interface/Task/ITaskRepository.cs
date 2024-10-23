using task_management.src.API.Base;
using task_management.src.API.Model;
namespace task_management.src.API.Interface
{
    public interface ITaskRepository : IRepository<TaskItem, int>
    {
        Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId);

        Task<(IEnumerable<TaskItem>, int)> GetPagedTasksByUserIdAsync(int userId, int pageNumber, int pageSize);
    }
    
}
