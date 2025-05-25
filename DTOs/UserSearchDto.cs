namespace UserDistributed.DTOs;

public record UserSearchDto(
    string Name = "",
    string Email = "",
    string? Phone = null,
    string? Address = null,
    string? City = null,
    string? State = null,
    string? Country = null,
    string? ZipCode = null,
    DateTime? CreatedAfter = null,
    DateTime? CreatedBefore = null
);