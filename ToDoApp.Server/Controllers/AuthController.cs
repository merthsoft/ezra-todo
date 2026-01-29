using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Server.Models;
using ToDoApp.Server.Services;

namespace ToDoApp.Server.Controllers;

[ApiController, 
    Route("api/[controller]")]
public class AuthController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            });
        }

        var token = tokenService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Success = true,
            Token = token,
            Email = user.Email
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,
                Errors = ["Invalid email or password."]
            });
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,
                Errors = ["Invalid email or password."]
            });
        }

        var token = tokenService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Success = true,
            Token = token,
            Email = user.Email
        });
    }
}
