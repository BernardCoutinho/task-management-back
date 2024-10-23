

using task_management.src.API.Base;
using task_management.src.API.Model;

namespace task_management.src.API.Interface
{
    public interface IUserRepository : IRepository<User, int>
    {
        Task<User> GetByUsernameOrEmailAsync(string username, string email);
    }
}
