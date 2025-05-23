using System;

namespace GeekMeet.DTOs
{
    public class UserStatisticsDto
    {
        public int TotalUsers { get; init; }
        public DateTime? LastUserCreated { get; init; }
    }
} 