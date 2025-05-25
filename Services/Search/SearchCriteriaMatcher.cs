using UserDistributed.DTOs;
using UserDistributed.Models;

namespace UserDistributed.Services.Search;

public class SearchCriteriaMatcher
{
    public async Task<bool> MatchesAsync(User user, UserSearchDto searchParams)
    {
        await Task.Delay(10); // Simulate some processing time

        return (string.IsNullOrEmpty(searchParams.Name) || user.Name.Contains(searchParams.Name)) &&
               (string.IsNullOrEmpty(searchParams.Email) || user.Email.Contains(searchParams.Email)) &&
               (string.IsNullOrEmpty(searchParams.City) || (user.City != null && user.City.Contains(searchParams.City))) &&
               (!searchParams.CreatedAfter.HasValue || user.CreatedAt >= searchParams.CreatedAfter) &&
               (!searchParams.CreatedBefore.HasValue || user.CreatedAt <= searchParams.CreatedBefore);
    }
}