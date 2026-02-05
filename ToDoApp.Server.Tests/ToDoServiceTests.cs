using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ToDoApp.Server.DataAccess;
using ToDoApp.Server.Models;
using ToDoApp.Server.Services;
using Xunit;

namespace ToDoApp.Server.Tests;

public class ToDoServiceTests : IDisposable
{
    private readonly ToDoDbContext _dbContext;
    private readonly ToDoService _service;
    private readonly ILogger<ToDoService> _logger;
    private const string TestUserId = "test-user-123";
    private const string OtherUserId = "other-user-456";

    public ToDoServiceTests()
    {
        var options = new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ToDoDbContext(options);
        _logger = Substitute.For<ILogger<ToDoService>>();
        _service = new ToDoService(_logger, _dbContext);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    #region GetAllTodosAsync Tests

    [Fact]
    public async Task GetAllTodosAsync_ReturnsEmptyList_WhenNoTodosExist()
    {
        // Act
        var result = await _service.GetAllTodosAsync(TestUserId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllTodosAsync_ReturnsOnlyUsersTodos()
    {
        // Arrange
        await SeedTodoAsync(TestUserId, "User's todo");
        await SeedTodoAsync(OtherUserId, "Other user's todo");

        // Act
        var result = await _service.GetAllTodosAsync(TestUserId);

        // Assert
        Assert.Single(result);
        Assert.Equal("User's todo", result.First().Title);
    }

    [Fact]
    public async Task GetAllTodosAsync_ReturnsTodosOrderedById()
    {
        // Arrange
        await SeedTodoAsync(TestUserId, "First");
        await SeedTodoAsync(TestUserId, "Second");
        await SeedTodoAsync(TestUserId, "Third");

        // Act
        var result = (await _service.GetAllTodosAsync(TestUserId)).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.True(result[0].Id < result[1].Id);
        Assert.True(result[1].Id < result[2].Id);
    }

    #endregion

    #region GetTodoByIdAsync Tests

    [Fact]
    public async Task GetTodoByIdAsync_ReturnsTodo_WhenExists()
    {
        // Arrange
        var todo = await SeedTodoAsync(TestUserId, "Test todo");

        // Act
        var result = await _service.GetTodoByIdAsync(TestUserId, todo.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(todo.Id, result.Id);
        Assert.Equal("Test todo", result.Title);
    }

    [Fact]
    public async Task GetTodoByIdAsync_ThrowsToDoNotFoundException_WhenTodoDoesNotExist()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ToDoNotFoundException>(
            () => _service.GetTodoByIdAsync(TestUserId, 999));
    }

    [Fact]
    public async Task GetTodoByIdAsync_ThrowsToDoNotFoundException_WhenTodoBelongsToOtherUser()
    {
        // Arrange
        var todo = await SeedTodoAsync(OtherUserId, "Other user's todo");

        // Act & Assert
        await Assert.ThrowsAsync<ToDoNotFoundException>(
            () => _service.GetTodoByIdAsync(TestUserId, todo.Id));
    }

    #endregion

    #region CreateTodoAsync Tests

    [Fact]
    public async Task CreateTodoAsync_CreatesTodo_WithValidRequest()
    {
        // Arrange
        var request = new CreateToDoRequest("New todo", false, DateTime.UtcNow.AddDays(1));

        // Act
        var result = await _service.CreateTodoAsync(TestUserId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New todo", result.Title);
        Assert.False(result.IsComplete);
        Assert.Equal(TestUserId, result.UserId);

        // Verify persisted
        var persisted = await _dbContext.Todos.FindAsync(result.Id);
        Assert.NotNull(persisted);
    }

    [Fact]
    public async Task CreateTodoAsync_ThrowsToDoServiceException_WhenTitleIsEmpty()
    {
        // Arrange
        var request = new CreateToDoRequest("");

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ToDoServiceException>(
            () => _service.CreateTodoAsync(TestUserId, request));
        Assert.Equal(ToDoServiceException.TitleEmptyError, ex.Message);
    }

    [Fact]
    public async Task CreateTodoAsync_ThrowsToDoServiceException_WhenTitleIsWhitespace()
    {
        // Arrange
        var request = new CreateToDoRequest("   ");

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ToDoServiceException>(
            () => _service.CreateTodoAsync(TestUserId, request));
        Assert.Equal(ToDoServiceException.TitleEmptyError, ex.Message);
    }

    [Fact]
    public async Task CreateTodoAsync_ThrowsToDoServiceException_WhenUserIdIsEmpty()
    {
        // Arrange
        var request = new CreateToDoRequest("Valid title");

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ToDoServiceException>(
            () => _service.CreateTodoAsync("", request));
        Assert.Equal(ToDoServiceException.UserIdEmptyError, ex.Message);
    }

    #endregion

    #region UpdateTodoAsync Tests

    [Fact]
    public async Task UpdateTodoAsync_UpdatesTodo_WithValidRequest()
    {
        // Arrange
        var todo = await SeedTodoAsync(TestUserId, "Original title");
        var request = new UpdateToDoRequest("Updated title", true, DateTime.UtcNow.AddDays(2), DateTime.UtcNow);

        // Act
        var result = await _service.UpdateTodoAsync(TestUserId, todo.Id, request);

        // Assert
        Assert.Equal("Updated title", result.Title);
        Assert.True(result.IsComplete);
        Assert.NotNull(result.CompletedOn);
    }

    [Fact]
    public async Task UpdateTodoAsync_ThrowsToDoNotFoundException_WhenTodoDoesNotExist()
    {
        // Arrange
        var request = new UpdateToDoRequest("Updated title", null);

        // Act & Assert
        await Assert.ThrowsAsync<ToDoNotFoundException>(
            () => _service.UpdateTodoAsync(TestUserId, 999, request));
    }

    [Fact]
    public async Task UpdateTodoAsync_ThrowsToDoNotFoundException_WhenTodoBelongsToOtherUser()
    {
        // Arrange
        var todo = await SeedTodoAsync(OtherUserId, "Other user's todo");
        var request = new UpdateToDoRequest("Trying to update", null);

        // Act & Assert
        await Assert.ThrowsAsync<ToDoNotFoundException>(
            () => _service.UpdateTodoAsync(TestUserId, todo.Id, request));
    }

    [Fact]
    public async Task UpdateTodoAsync_ThrowsToDoServiceException_WhenTitleIsEmpty()
    {
        // Arrange
        var todo = await SeedTodoAsync(TestUserId, "Original title");
        var request = new UpdateToDoRequest("", null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ToDoServiceException>(
            () => _service.UpdateTodoAsync(TestUserId, todo.Id, request));
        Assert.Equal(ToDoServiceException.TitleEmptyError, ex.Message);
    }

    [Fact]
    public async Task UpdateTodoAsync_ThrowsToDoServiceException_WhenCompletedWithoutDate()
    {
        // Arrange
        var todo = await SeedTodoAsync(TestUserId, "Title");
        var request = new UpdateToDoRequest("Title", true);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ToDoServiceException>(
            () => _service.UpdateTodoAsync(TestUserId, todo.Id, request));
        Assert.Equal(ToDoServiceException.CompletedOnRequiredError, ex.Message);
    }

    [Fact]
    public async Task UpdateTodoAsync_ClearsCompletedOn_WhenMarkingIncomplete()
    {
        // Arrange
        var todo = await SeedTodoAsync(TestUserId, "Completed todo", true, DateTime.UtcNow);
        var request = new UpdateToDoRequest("Still completed todo", false);

        // Act
        var result = await _service.UpdateTodoAsync(TestUserId, todo.Id, request);

        // Assert
        Assert.False(result.IsComplete);
        Assert.Null(result.CompletedOn);
    }

    #endregion

    #region DeleteTodoAsync Tests

    [Fact]
    public async Task DeleteTodoAsync_DeletesTodo_WhenExists()
    {
        // Arrange
        var todo = await SeedTodoAsync(TestUserId, "To be deleted");

        // Act
        await _service.DeleteTodoAsync(TestUserId, todo.Id);

        // Assert
        var deleted = await _dbContext.Todos.FindAsync(todo.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteTodoAsync_ThrowsToDoNotFoundException_WhenTodoDoesNotExist()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ToDoNotFoundException>(
            () => _service.DeleteTodoAsync(TestUserId, 999));
    }

    [Fact]
    public async Task DeleteTodoAsync_ThrowsToDoNotFoundException_WhenTodoBelongsToOtherUser()
    {
        // Arrange
        var todo = await SeedTodoAsync(OtherUserId, "Other user's todo");

        // Act & Assert
        await Assert.ThrowsAsync<ToDoNotFoundException>(
            () => _service.DeleteTodoAsync(TestUserId, todo.Id));
    }

    #endregion

    #region Helper Methods

    private async Task<ToDoItem> SeedTodoAsync(
        string userId, 
        string title, 
        bool isComplete = false, 
        DateTime? completedOn = null)
    {
        var todo = new ToDoItem
        {
            Title = title,
            UserId = userId,
            IsComplete = isComplete,
            CompletedOn = completedOn
        };

        _dbContext.Todos.Add(todo);
        await _dbContext.SaveChangesAsync();
        return todo;
    }

    #endregion
}
