namespace ToDoApp.Server.Models;

public class ToDoItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool IsComplete { get; set; }
    public DateTime? CompleteBy { get; set; }
    public DateTime? CompletedOn { get; set; }
    
    public required string UserId { get; set; }
    public User User { get; set; } = null!;
}
