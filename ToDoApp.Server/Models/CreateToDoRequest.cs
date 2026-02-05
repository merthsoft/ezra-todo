using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Server.Models;

public record CreateToDoRequest(
    [Required(AllowEmptyStrings = false)] string Title, 
    bool IsComplete = false, 
    DateTime? CompleteBy = null);
