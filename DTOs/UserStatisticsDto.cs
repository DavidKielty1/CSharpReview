using System;

namespace UserDistributed.DTOs;

public record UserStatisticsDto(
    int TotalUsers,
    DateTime? LastUserCreated
);