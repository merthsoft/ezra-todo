namespace ToDoApp.Server.Services;

[Serializable]
public class ToDoServiceException : Exception
{
    public const string TitleEmptyError = "Request title cannot be null or empty.";
    public const string UserIdEmptyError = "User ID cannot be null or empty.";
    public const string InvalidIdError = "Invalid ID supplied.";
    public const string CompletedOnRequiredError = "CompletedOn date must be provided when marking a todo as complete.";
    
    public ToDoServiceException() { }
    public ToDoServiceException(string message) : base(message) { }
    public ToDoServiceException(string message, Exception inner) : base(message, inner) { }
}

[Serializable]
public class ToDoNotFoundException : ToDoServiceException
{
    public const string TodoNotFoundError = "Todo item not found.";

    public ToDoNotFoundException() : base(TodoNotFoundError) { }
}
