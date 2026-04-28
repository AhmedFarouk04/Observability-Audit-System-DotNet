namespace Api.Authentication;

public sealed class TokenResponse
{
    public string AccessToken { get; init; } = string.Empty;

    public DateTime ExpiresAtUtc { get; init; }

    public string TokenType { get; init; } = "Bearer";
}
