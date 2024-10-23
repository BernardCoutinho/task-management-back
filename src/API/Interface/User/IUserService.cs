using task_management.src.API.Base;
using task_management.src.API.Model;
using task_management.src.API.View.User;

namespace task_management.src.API.Interface
{
    public interface IUserService : IService<User, int>
    {
        Task<User> CreateUserAsync(UserRequest request);

        Task<User> GetByUsernameOrEmailAsync(string username, string email);
    }
}
