using System;

namespace GeekMeet.DTOs;

public record UserStatisticsDto(
    int TotalUsers,
    DateTime? LastUserCreated
); 