namespace Api.Authentication;

public interface ITokenService
{
    TokenResponse CreateToken(TokenRequest request);
}
