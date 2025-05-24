using GeekMeet.Models;

namespace GeekMeet.Services.Metrics.Calculations;

public class ActivityScoreCalculator
{
    public double Calculate(User user)
    {
        var baseScore = 100.0;
        var daysSinceCreation = (DateTime.UtcNow - user.CreatedAt).TotalDays;
        var cityBonus = user.City?.ToLower() switch
        {
            "new york" => 20.0,
            "los angeles" => 15.0,
            "chicago" => 10.0,
            _ => 5.0
        };
        
        return baseScore + cityBonus - (daysSinceCreation * 0.5);
    }
}

public class EngagementRateCalculator
{
    public double Calculate(User user)
    {
        var baseRate = 0.5;
        var emailFactor = user.Email.Contains("@example.com") ? 0.2 : 0.1;
        var cityFactor = user.City != null ? 0.3 : 0.1;
        
        return baseRate + emailFactor + cityFactor;
    }
}

public class PerformanceIndexCalculator
{
    public double Calculate(User user)
    {
        var baseIndex = 1.0;
        var nameLengthFactor = user.Name.Length * 0.05;
        var cityFactor = user.City?.Length * 0.02 ?? 0;
        
        return baseIndex + nameLengthFactor + cityFactor;
    }
} 