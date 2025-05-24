using System.ComponentModel.DataAnnotations;

namespace GeekMeet.DTOs;

public record UserDto(
    int Id,
    string Name,
    string Email,
    DateTime CreatedAt
);

public record CreateUserDto(
    [Required]
    [StringLength(100)]
    string Name,

    [Required]
    [EmailAddress]
    [StringLength(255)]
    string Email,

    [Required]
    [StringLength(100, MinimumLength = 8)]
    string Password
);

public record UpdateUserDto(
    [StringLength(100)]
    string? Name = null,

    [EmailAddress]
    [StringLength(255)]
    string? Email = null,

    [StringLength(100, MinimumLength = 8)]
    string? Password = null
); 