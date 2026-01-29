using Microsoft.AspNetCore.Identity;

namespace ToDoApp.Server.Models;

public class User : IdentityUser
{
    public ICollection<ToDoItem> Todos { get; set; } = [];
}
