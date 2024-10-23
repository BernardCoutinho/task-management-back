namespace task_management.src.API.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }

        public ICollection<TaskItem> Tasks { get; set; }

    }
}
