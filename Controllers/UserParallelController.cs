using UserDistributed.DTOs;
using UserDistributed.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserDistributed.Controllers;

// [Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserParallelController(
    IUserParallelService userParallelService,
    IUserService userService) : ControllerBase
{
    [HttpPost("process")]
    public async Task<ActionResult<IEnumerable<UserDto>>> ProcessUsersInParallel()
    {
        var users = await userService.GetAllUsersAsync();
        var processedUsers = await userParallelService.UserProcessor.ProcessUsersAsync(users);
        return Ok(processedUsers);
    }

    [HttpGet("statistics/city")]
    public async Task<ActionResult<Dictionary<string, int>>> GetUserStatisticsByCity()
    {
        var statistics = await userParallelService.CityStatsService.GetStatisticsAsync();
        return Ok(statistics);
    }

    [HttpGet("search/parallel")]
    public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsersParallel([FromQuery] UserSearchDto searchParams)
    {
        var users = await userParallelService.SearchService.SearchUsersAsync(searchParams);
        return Ok(users);
    }

    [HttpGet("metrics")]
    public async Task<ActionResult<Dictionary<string, double>>> GetUserMetrics()
    {
        var metrics = await userParallelService.MetricsService.CalculateMetricsAsync();
        return Ok(metrics);
    }
}