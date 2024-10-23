using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using task_management.src.API.Base.Context;
using task_management.src.API.Interface;
using task_management.src.API.Model;

namespace task_management.src.API.Repository
{
   public class TaskRepository : ITaskRepository
{
        private readonly PrincipalDbContext _context;

        public TaskRepository(PrincipalDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<TaskItem>, int)> GetPagedTasksByUserIdAsync(int userId, int pageNumber, int pageSize)
        {
            var query =  _context.Tasks.Where(t => t.UserId == userId);

            int totalItems = await query.CountAsync();
            var queryResult = await query.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (queryResult, totalItems); 
        }


        public async Task<TaskItem> AddAsync(TaskItem task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteAsync(TaskItem task)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var task = await GetByIdAsync(id);
            if (task is not null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.Tasks.FirstOrDefaultAsync(post => post.Id == id);
        }

        public async Task<TaskItem> UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId)
        {
            return await _context.Tasks.Where(t => t.UserId == userId).ToListAsync();
        }
    }
}
