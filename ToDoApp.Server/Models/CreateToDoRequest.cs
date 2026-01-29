namespace ToDoApp.Server.Models;

public record CreateToDoRequest(
    string Title, 
    bool IsComplete = false, 
    DateTime? CompleteBy = null);
