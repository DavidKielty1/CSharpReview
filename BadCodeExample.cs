using Microsoft.AspNetCore.Mvc;
using UserDistributed.Data;
using UserDistributed.Models;
using Microsoft.EntityFrameworkCore;

namespace UserDistributed.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly string _apiKey = "1234567890abcdef"; // Hardcoded API key.

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        try
        {
            // Anti-pattern: Using ToList() instead of ToListAsync()
            var users = _context.Users.ToList();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("users/{id}")]
    public IActionResult GetUserById(int id)
    {
        try
        {
            // Anti-pattern: Not using async/await
            var user = _context.Users.Find(id);
            if (user is null)
                return NotFound();
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("users")]
    public IActionResult CreateUser([FromBody] dynamic userData)
    {
        try
        {
            // Anti-pattern: No validation
            var user = new User
            {
                Name = userData.Name.ToString(),
                Email = userData.Email.ToString(),
                PasswordHash = userData.Password.ToString(), // Still storing plain text
                CreatedAt = DateTime.Now
            };

            // Anti-pattern: Not using async/await
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(new { message = "User created successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("users/{id}")]
    public IActionResult UpdateUser(int id, [FromBody] dynamic userData)
    {
        try
        {
            // Anti-pattern: Not using async/await
            var user = _context.Users.Find(id);
            if (user is null)
                return NotFound();

            user.Name = userData.Name.ToString();
            user.Email = userData.Email.ToString();
            user.PasswordHash = userData.Password.ToString();

            _context.SaveChanges();
            return Ok(new { message = "User updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("users/{id}")]
    public IActionResult DeleteUser(int id)
    {
        try
        {
            // Anti-pattern: Not using async/await
            var user = _context.Users.Find(id);
            if (user is null)
                return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok(new { message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("users/search")]
    public IActionResult SearchUsers(string query)
    {
        try
        {
            // Anti-pattern: Client-side filtering
            var allUsers = _context.Users.ToList();
            var filteredUsers = allUsers.Where(u =>
                u.Name.Contains(query) ||
                u.Email.Contains(query)
            ).ToList();
            return Ok(filteredUsers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("users/statistics")]
    public IActionResult GetUserStatistics()
    {
        try
        {
            // Anti-pattern: Multiple separate queries
            var totalUsers = _context.Users.Count();
            var lastUserCreated = _context.Users.Max(u => u.CreatedAt);

            return Ok(new { TotalUsers = totalUsers, LastUserCreated = lastUserCreated });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Anti-pattern: N+1 queries
    public async Task<List<User>> GetUsersWithNPlusOne()
    {
        var users = await _context.Users.ToListAsync();
        foreach (var user in users)
        {
            // This creates a separate query for each user
            user.CreatedAt = await _context.Users
                .Where(u => u.Id == user.Id)
                .Select(u => u.CreatedAt)
                .FirstOrDefaultAsync();
        }
        return users;
    }

    // Anti-pattern: Not using async/await properly
    public List<User> GetUsersSync()
    {
        // Blocking call in async context
        return _context.Users.ToList();
    }

    // Anti-pattern: Not disposing DbContext
    public async Task CreateUserWithoutDispose(User user)
    {
        var context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
        context.Users.Add(user);
        await context.SaveChangesAsync();
        // Context not disposed
    }

    // Anti-pattern: Not using transactions
    public async Task UpdateUsersWithoutTransaction(List<User> users)
    {
        foreach (var user in users)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(); // Each save is a separate transaction
        }
    }

    // Anti-pattern: Not using proper tracking
    public async Task UpdateUserWithWrongTracking(User user)
    {
        var existingUser = await _context.Users.FindAsync(user.Id);
        if (existingUser != null)
        {
            _context.Entry(existingUser).State = EntityState.Detached; // Wrong: detaching entity
            _context.Users.Update(user); // Creates new tracking
            await _context.SaveChangesAsync();
        }
    }

    // Anti-pattern: Not using proper includes
    public async Task<User?> GetUserWithoutIncludes(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            // Lazy loading without proper configuration
            var relatedData = user.City; // This might trigger N+1 queries
        }
        return user;
    }

    // Anti-pattern: Not using proper query filtering
    public async Task<List<User>> GetUsersWithClientSideFiltering()
    {
        var allUsers = await _context.Users.ToListAsync();
        return allUsers.Where(u => u.City == "London").ToList(); // Filtering in memory
    }
}
