using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Server.Models;

public class RegisterRequest
{
    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}
