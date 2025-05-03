using System;
using System.ComponentModel.DataAnnotations;

namespace GeekMeet.DTOs
{
    public class UserDto
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public required string Email { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public class CreateUserDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; init; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public required string Email { get; init; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public required string Password { get; init; }
    }

    public class UpdateUserDto
    {
        [StringLength(100)]
        public string? Name { get; init; }

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; init; }

        [StringLength(100, MinimumLength = 8)]
        public string? Password { get; init; }
    }
} 