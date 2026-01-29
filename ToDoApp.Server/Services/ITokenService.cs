using ToDoApp.Server.Models;

namespace ToDoApp.Server.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
