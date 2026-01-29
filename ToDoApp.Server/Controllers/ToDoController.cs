using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Server.Models;
using ToDoApp.Server.Services;

namespace ToDoApp.Server.Controllers;

[ApiController, 
    Authorize, 
    Route("api/[controller]")]
public class ToDoController(IToDoService toDoService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) 
        ?? throw new UnauthorizedAccessException("User ID not found in claims.");

    [HttpGet]
    public async Task<IActionResult> GetAllTodos(CancellationToken cancellationToken = default)
        => Ok(await toDoService.GetAllTodosAsync(UserId, cancellationToken));

    [HttpGet("{id:int}", Name = "GetTodoById")]
    public async Task<IActionResult> GetTodoById(int id, CancellationToken cancellationToken = default)
    {
        var todo = await toDoService.GetTodoByIdAsync(UserId, id, cancellationToken);
        return todo is not null ? Ok(todo) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreateTodo(CreateToDoRequest request, CancellationToken cancellationToken = default)
    {
        var todo = await toDoService.CreateTodoAsync(UserId, request, cancellationToken);
        return CreatedAtRoute("GetTodoById", new { id = todo.Id }, todo);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTodo(int id, UpdateToDoRequest request, CancellationToken cancellationToken = default)
    {
        var todo = await toDoService.UpdateTodoAsync(UserId, id, request, cancellationToken);
        return todo is not null ? Ok(todo) : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTodo(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await toDoService.DeleteTodoAsync(UserId, id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
