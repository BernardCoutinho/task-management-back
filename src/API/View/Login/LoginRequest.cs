namespace task_management.src.API.View.Login
{
    public record LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
