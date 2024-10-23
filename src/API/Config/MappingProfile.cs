using AutoMapper;
using task_management.src.API.Model;
using task_management.src.API.View;
using task_management.src.API.View.Task;
using task_management.src.API.View.User;
namespace task_management.src.API.Config
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<TaskItemRequest, TaskItem>();
            CreateMap<TaskItem, TaskItemRequest>();
            CreateMap<TaskItem, TaskItemResponse>();
            CreateMap<TaskItemResponse, TaskItem>();
            CreateMap<User, UserResponse>();
            CreateMap<UserResponse, User>();
        }
    }
}
