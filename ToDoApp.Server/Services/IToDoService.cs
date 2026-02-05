using ToDoApp.Server.Models;

namespace ToDoApp.Server.Services;

/// <summary>
/// Service interface for managing TODO items.
/// </summary>
public interface IToDoService
{
    /// <summary>
    /// Gets all TODO items for a user ordered by ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    Task<IEnumerable<ToDoItem>> GetAllTodosAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a TODO item by its ID for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="id">The ID of the TODO item.</param>
    /// <returns>The TODO item if found.</returns>
    Task<ToDoItem> GetTodoByIdAsync(string userId, int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new TODO item for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="request">The request containing TODO item details.</param>
    /// <returns>The created TODO item.</returns>
    Task<ToDoItem> CreateTodoAsync(string userId, CreateToDoRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing TODO item for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="id">The ID of the TODO item to update.</param>
    /// <param name="request">The request containing updated TODO item details.</param>
    /// <returns>The updated TODO item if found.</returns>
    Task<ToDoItem> UpdateTodoAsync(string userId, int id, UpdateToDoRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a TODO item by its ID for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="id">The ID of the TODO item to delete.</param>
    Task DeleteTodoAsync(string userId, int id, CancellationToken cancellationToken = default);
}
