using GeekMeet.DTOs;
using GeekMeet.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekMeet.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await userService.GetUserByIdAsync(id);
        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto userToCreate)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdUser = await userService.CreateUserAsync(userToCreate);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserDto userToUpdate)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await userService.UpdateUserAsync(id, userToUpdate);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await userService.DeleteUserAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsers([FromQuery] UserSearchDto searchParams)
    {
        var users = await userService.SearchUsersAsync(searchParams);
        return Ok(users);
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<UserStatisticsDto>> GetUserStatistics()
    {
        var stats = await userService.GetUserStatisticsAsync();
        return Ok(new UserStatisticsDto
        {
            TotalUsers = stats.totalUsers,
            LastUserCreated = stats.lastUserCreated
        });
    }
} 