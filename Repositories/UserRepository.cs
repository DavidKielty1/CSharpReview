using System.Data.SqlClient;
using Dapper;
using GeekMeet.Interfaces;
using GeekMeet.Models;

namespace GeekMeet.Repositories;

public class UserRepository(IConfiguration configuration) : IUserRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<User>("SELECT Id, Name, Email, PasswordHash, CreatedAt FROM Users");
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT Id, Name, Email, PasswordHash, CreatedAt FROM Users WHERE Id = @Id",
            new { Id = id });
    }

    public async Task<User> CreateAsync(User user)
    {
        await using var connection = new SqlConnection(_connectionString);
        var sql = @"
            INSERT INTO Users (Name, Email, PasswordHash, CreatedAt)
            VALUES (@Name, @Email, @PasswordHash, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int)";

        user.Id = await connection.QuerySingleAsync<int>(sql, user);
        return user;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        await using var connection = new SqlConnection(_connectionString);
        var rowsAffected = await connection.ExecuteAsync(
            "UPDATE Users SET Name = @Name, Email = @Email, PasswordHash = @PasswordHash WHERE Id = @Id",
            user);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        var rowsAffected = await connection.ExecuteAsync(
            "DELETE FROM Users WHERE Id = @Id",
            new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<User>> SearchAsync(string query)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<User>(
            "SELECT Id, Name, Email, PasswordHash, CreatedAt FROM Users WHERE Name LIKE @Query OR Email LIKE @Query",
            new { Query = $"%{query}%" });
    }

    public async Task<(int totalUsers, DateTime lastUserCreated)> GetStatisticsAsync()
    {
        await using var connection = new SqlConnection(_connectionString);
        var result = await connection.QueryFirstAsync<(int, DateTime)>(
            "SELECT COUNT(*) as TotalUsers, MAX(CreatedAt) as LastUserCreated FROM Users");
        return result;
    }
} 