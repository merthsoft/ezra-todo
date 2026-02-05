using Microsoft.EntityFrameworkCore;
using ToDoApp.Server.DataAccess;
using ToDoApp.Server.Models;

namespace ToDoApp.Server.Services;

public class ToDoService(
    ILogger<ToDoService> logger,
    ToDoDbContext db) : IToDoService
{
    public async Task<IEnumerable<ToDoItem>> GetAllTodosAsync(string userId, CancellationToken cancellationToken = default) 
        => await db.Todos
            .Where(t => t.UserId == userId)
            .OrderBy(t => t.Id)
            .ToListAsync(cancellationToken: cancellationToken);

    public async Task<ToDoItem> GetTodoByIdAsync(string userId, int id, CancellationToken cancellationToken = default)
    {
        var todo = await db.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken: cancellationToken);

        return todo ?? throw new ToDoNotFoundException();
    }

    public async Task<ToDoItem> CreateTodoAsync(string userId, CreateToDoRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ToDoServiceException(ToDoServiceException.TitleEmptyError);

        if (string.IsNullOrWhiteSpace(userId))
            throw new ToDoServiceException(ToDoServiceException.UserIdEmptyError);

        var todo = new ToDoItem
        {
            Title = request.Title,
            IsComplete = request.IsComplete,
            CompleteBy = request.CompleteBy,
            UserId = userId
        };

        db.Todos.Add(todo);
        await db.SaveChangesAsync(cancellationToken);

        return todo;
    }

    public async Task<ToDoItem> UpdateTodoAsync(string userId, int id, UpdateToDoRequest request, CancellationToken cancellationToken = default)
    {
        var existingTodo = await db.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken: cancellationToken);

        if (existingTodo is null)
        {
            logger.LogError("Todo item with ID {Id} for user {UserId} not found.", id, userId);
            throw new ToDoNotFoundException();
        }

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ToDoServiceException(ToDoServiceException.TitleEmptyError);

        if (string.IsNullOrWhiteSpace(userId))
            throw new ToDoServiceException(ToDoServiceException.UserIdEmptyError);

        if (request.IsComplete == true && request.CompletedOn == null)
            throw new ToDoServiceException(ToDoServiceException.CompletedOnRequiredError);

        existingTodo.Title = request.Title ?? existingTodo.Title;
        existingTodo.IsComplete = request.IsComplete ?? existingTodo.IsComplete;
        existingTodo.CompleteBy = request.CompleteBy ?? existingTodo.CompleteBy;
        existingTodo.CompletedOn = !(request.IsComplete ?? existingTodo.IsComplete)
                                    ? null
                                    : request.CompletedOn ?? existingTodo.CompletedOn;

        await db.SaveChangesAsync(cancellationToken);
        return existingTodo;
    }

    public async Task DeleteTodoAsync(string userId, int id, CancellationToken cancellationToken = default)
    {
        var existingTodo = await db.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken: cancellationToken);

        if (existingTodo is null)
        {
            logger.LogError("Todo item with ID {Id} for user {UserId} not found.", id, userId);
            throw new ToDoNotFoundException();
        }

        if (string.IsNullOrWhiteSpace(userId))
            throw new ToDoServiceException(ToDoServiceException.UserIdEmptyError);

        db.Todos.Remove(existingTodo);
        await db.SaveChangesAsync(cancellationToken);
    }
}
