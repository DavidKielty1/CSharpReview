using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace GeekMeet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IConfiguration configuration) : ControllerBase
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Server=.;Database=GeekMeet;Trusted_Connection=True;"; // Hardcoded fallback
    private readonly string _apiKey = "1234567890abcdef"; // Hardcoded API key

    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        try
        {
            var users = new List<dynamic>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand("SELECT * FROM Users", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new
                {
                    Id = reader["Id"],
                    Name = reader["Name"],
                    Email = reader["Email"],
                    Password = reader["Password"], // Still storing plain text passwords
                    CreatedAt = reader["CreatedAt"]
                });
            }
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
            dynamic? user = null;
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand($"SELECT * FROM Users WHERE Id = {id}", connection); // Still vulnerable to SQL injection
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                user = new
                {
                    Id = reader["Id"],
                    Name = reader["Name"],
                    Email = reader["Email"],
                    Password = reader["Password"],
                    CreatedAt = reader["CreatedAt"]
                };
            }
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
            // Still no validation
            var name = userData.Name.ToString();
            var email = userData.Email.ToString();
            var password = userData.Password.ToString(); // Still storing plain text

            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand($"INSERT INTO Users (Name, Email, Password, CreatedAt) VALUES ('{name}', '{email}', '{password}', GETDATE())", connection);
            command.ExecuteNonQuery();
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
            var name = userData.Name.ToString();
            var email = userData.Email.ToString();
            var password = userData.Password.ToString();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand($"UPDATE Users SET Name = '{name}', Email = '{email}', Password = '{password}' WHERE Id = {id}", connection);
            command.ExecuteNonQuery();
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
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand($"DELETE FROM Users WHERE Id = {id}", connection);
            command.ExecuteNonQuery();
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
            var users = new List<dynamic>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand($"SELECT * FROM Users WHERE Name LIKE '%{query}%' OR Email LIKE '%{query}%'", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new
                {
                    Id = reader["Id"],
                    Name = reader["Name"],
                    Email = reader["Email"],
                    Password = reader["Password"],
                    CreatedAt = reader["CreatedAt"]
                });
            }
            return Ok(users);
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
            var stats = new { TotalUsers = 0, LastUserCreated = DateTime.MinValue };
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand("SELECT COUNT(*) as TotalUsers, MAX(CreatedAt) as LastUserCreated FROM Users", connection);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                stats = new
                {
                    TotalUsers = Convert.ToInt32(reader["TotalUsers"]),
                    LastUserCreated = Convert.ToDateTime(reader["LastUserCreated"])
                };
            }
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
