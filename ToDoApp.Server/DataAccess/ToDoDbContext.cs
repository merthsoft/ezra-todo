using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Server.Models;

namespace ToDoApp.Server.DataAccess;

public class ToDoDbContext(DbContextOptions<ToDoDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<ToDoItem> Todos => Set<ToDoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<ToDoItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Id);
            entity.HasIndex(e => e.UserId);

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.UserId).IsRequired();

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Todos)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
