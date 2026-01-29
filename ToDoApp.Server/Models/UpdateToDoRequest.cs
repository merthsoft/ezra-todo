namespace ToDoApp.Server.Models;

public record UpdateToDoRequest(
    string? Title, 
    bool? IsComplete, 
    DateTime? CompleteBy = null, 
    DateTime? CompletedOn = null);
