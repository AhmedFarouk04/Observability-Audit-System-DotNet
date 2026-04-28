namespace Api.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "ObservabilityAuditSystem";

    public string Audience { get; init; } = "ObservabilityAuditSystem.Client";

    public string SigningKey { get; init; } = "THIS_IS_A_DEVELOPMENT_ONLY_SIGNING_KEY_CHANGE_ME_12345";

    public int ExpiresMinutes { get; init; } = 60;
}
