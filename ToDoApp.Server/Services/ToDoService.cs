using Microsoft.EntityFrameworkCore;
using ToDoApp.Server.DataAccess;
using ToDoApp.Server.Models;

namespace ToDoApp.Server.Services;

public class ToDoService(ToDoDbContext db) : IToDoService
{
    public async Task<IEnumerable<ToDoItem>> GetAllTodosAsync(string userId, CancellationToken cancellationToken = default) 
        => await db.Todos
            .Where(t => t.UserId == userId)
            .OrderBy(t => t.Id)
            .ToListAsync(cancellationToken: cancellationToken);

    public async Task<ToDoItem?> GetTodoByIdAsync(string userId, int id, CancellationToken cancellationToken = default)
        => await db.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken: cancellationToken);

    public async Task<ToDoItem> CreateTodoAsync(string userId, CreateToDoRequest request, CancellationToken cancellationToken = default)
    {
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

    public async Task<ToDoItem?> UpdateTodoAsync(string userId, int id, UpdateToDoRequest request, CancellationToken cancellationToken = default)
    {
        var existingTodo = await db.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken: cancellationToken);
        
        if (existingTodo is null)
            return null;

        existingTodo.Title = request.Title ?? existingTodo.Title;
        existingTodo.IsComplete = request.IsComplete ?? existingTodo.IsComplete;
        existingTodo.CompleteBy = request.CompleteBy ?? existingTodo.CompleteBy;
        existingTodo.CompletedOn = !(request.IsComplete ?? existingTodo.IsComplete)
                                    ? null
                                    : request.CompletedOn ?? existingTodo.CompletedOn;

        await db.SaveChangesAsync(cancellationToken);

        return existingTodo;
    }

    public async Task<bool> DeleteTodoAsync(string userId, int id, CancellationToken cancellationToken = default)
    {
        var todo = await db.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken: cancellationToken);
        
        if (todo is null)
            return false;

        db.Todos.Remove(todo);
        await db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
