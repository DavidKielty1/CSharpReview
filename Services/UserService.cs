using AutoMapper;
using UserDistributed.DTOs;
using UserDistributed.Interfaces;
using UserDistributed.Models;

namespace UserDistributed.Services;

public class UserService(
    IUserRepository userRepository,
    IMapper mapper,
    ILogger<UserService> logger) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        try
        {
            var users = await userRepository.GetAllAsync();
            return mapper.Map<IEnumerable<UserDto>>(users);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all users");
            throw;
        }
    }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(id);
            return mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user with id {UserId}", id);
            throw;
        }
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto userToCreate)
    {
        try
        {
            var user = mapper.Map<User>(userToCreate);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userToCreate.Password);
            user.CreatedAt = DateTime.UtcNow;

            var createdUser = await userRepository.CreateAsync(user);
            return mapper.Map<UserDto>(createdUser);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user");
            throw;
        }
    }

    public async Task<bool> UpdateUserAsync(int id, UpdateUserDto userToUpdate)
    {
        try
        {
            var existingUser = await userRepository.GetByIdAsync(id);
            if (existingUser is null)
                return false;

            mapper.Map(userToUpdate, existingUser);
            if (!string.IsNullOrEmpty(userToUpdate.Password))
            {
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userToUpdate.Password);
            }

            return await userRepository.UpdateAsync(existingUser);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user with id {UserId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        try
        {
            return await userRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting user with id {UserId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<UserDto>> SearchUsersAsync(UserSearchDto searchParams)
    {
        try
        {
            var users = await userRepository.SearchAsync(searchParams);
            return mapper.Map<IEnumerable<UserDto>>(users);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching users with parameters {@SearchParams}", searchParams);
            throw;
        }
    }

    public async Task<(int totalUsers, DateTime? lastUserCreated)> GetUserStatisticsAsync()
    {
        try
        {
            return await userRepository.GetStatisticsAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user statistics");
            throw;
        }
    }
}