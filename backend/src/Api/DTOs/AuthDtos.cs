namespace Api.DTOs;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, string RefreshToken, UserDto User);
public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
public record RefreshTokenRequest(string RefreshToken);

public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public Guid? BranchId { get; init; }
}
