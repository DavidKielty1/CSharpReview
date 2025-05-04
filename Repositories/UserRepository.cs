using GeekMeet.Data;
using GeekMeet.DTOs;
using GeekMeet.Interfaces;
using GeekMeet.Models;
using Microsoft.EntityFrameworkCore;

namespace GeekMeet.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;
        
        _context.Users.Remove(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<User>> SearchAsync(UserSearchDto searchParams)
    {
        return await _context.Users
            .Where(u => string.IsNullOrEmpty(searchParams.Name) || u.Name.Contains(searchParams.Name))
            .Where(u => string.IsNullOrEmpty(searchParams.Email) || u.Email.Contains(searchParams.Email))
            .Where(u => string.IsNullOrEmpty(searchParams.City) || (u.City != null && u.City.Contains(searchParams.City)))
            .Where(u => !searchParams.CreatedAfter.HasValue || u.CreatedAt >= searchParams.CreatedAfter)
            .Where(u => !searchParams.CreatedBefore.HasValue || u.CreatedAt <= searchParams.CreatedBefore)
            .ToListAsync();
    }

    public async Task<(int totalUsers, DateTime? lastUserCreated)> GetStatisticsAsync()
    {
        var totalUsers = await _context.Users.CountAsync();
        var lastUserCreated = await _context.Users.MaxAsync(u => (DateTime?)u.CreatedAt);
        return (totalUsers, lastUserCreated);
    }
} 