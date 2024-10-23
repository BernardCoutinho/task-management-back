namespace task_management.src.API.View.User
{
    public record UserRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
