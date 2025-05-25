using UserDistributed.Models;
using Microsoft.EntityFrameworkCore;

namespace UserDistributed.Data;

public static class DatabaseSeeder
{
    public static async Task SeedData(ApplicationDbContext context)
    {
        if (!await context.Users.AnyAsync())
        {
            var users = new List<User>
            {
                new User
                {
                    Name = "John Doe",
                    Email = "john@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    City = "New York",
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new User
                {
                    Name = "Jane Smith",
                    Email = "jane@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    City = "Los Angeles",
                    CreatedAt = DateTime.UtcNow.AddDays(-25)
                },
                new User
                {
                    Name = "Bob Johnson",
                    Email = "bob@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    City = "Chicago",
                    CreatedAt = DateTime.UtcNow.AddDays(-20)
                },
                new User
                {
                    Name = "Alice Brown",
                    Email = "alice@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    City = "New York",
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                },
                new User
                {
                    Name = "Charlie Wilson",
                    Email = "charlie@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    City = "Los Angeles",
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}