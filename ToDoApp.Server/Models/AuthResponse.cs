namespace ToDoApp.Server.Models;

public class AuthResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Email { get; set; }
    public List<string> Errors { get; set; } = [];
}
