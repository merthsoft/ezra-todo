using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ToDoApp.Server;
using ToDoApp.Server.Configuration;
using ToDoApp.Server.DataAccess;
using ToDoApp.Server.Models;
using ToDoApp.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddProblemDetails()
    .AddControllers();

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=todos.db"));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 1;
    }
    else
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
    }
    
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ToDoDbContext>()
.AddDefaultTokenProviders();

var jwtSection = builder.Configuration.GetSection(JwtSettings.SectionName)
    ?? throw new ArgumentException($"Could not get config section {JwtSettings.SectionName}");

var jwtSettings = jwtSection.Get<JwtSettings>()
    ?? throw new ArgumentException($"Could not read JWT settings from {JwtSettings.SectionName}");

builder.Services
    .Configure<JwtSettings>(jwtSection)
    .AddSingleton(jwtSettings);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

builder.Services
    .AddAuthorization()
    .AddScoped<IToDoService, ToDoService>()
    .AddScoped<ITokenService, TokenService>()
    .AddOpenApi();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
db.Database.EnsureCreated();

if (app.Environment.IsDevelopment())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var testEmail = "user@test.com";
    var existingUser = await userManager.FindByEmailAsync(testEmail);

    if (existingUser is null)
    {
        var testUser = new User
        {
            UserName = testEmail,
            Email = testEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(testUser, "Password123!");
    }
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();

app.MapDefaultEndpoints();

app.UseFileServer();

app.Run();
