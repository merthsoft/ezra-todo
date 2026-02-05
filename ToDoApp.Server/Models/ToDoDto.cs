namespace ToDoApp.Server.Models;

public class ToDoDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool IsComplete { get; set; }
    public DateTime? CompleteBy { get; set; }
    public DateTime? CompletedOn { get; set; }
}
