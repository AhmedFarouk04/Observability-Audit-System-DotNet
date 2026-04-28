using SharedKernel.Interfaces;
using System.Security.Claims;

namespace Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId =>
        FindClaim(ClaimTypes.NameIdentifier)
        ?? FindClaim("sub");

    public string? Email =>
        FindClaim(ClaimTypes.Email)
        ?? FindClaim("email");

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    private string? FindClaim(string type)
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(type)?.Value;
    }
}
