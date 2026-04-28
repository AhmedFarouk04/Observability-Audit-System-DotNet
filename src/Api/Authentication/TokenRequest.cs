namespace Api.Authentication;

public sealed class TokenRequest
{
    public string UserId { get; init; } = "dev-user";

    public string Email { get; init; } = "dev-user@example.com";

    public string Role { get; init; } = "admin";
}
