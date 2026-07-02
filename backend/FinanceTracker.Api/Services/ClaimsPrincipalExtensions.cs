using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinanceTracker.Api.Services;

public static class ClaimsPrincipalExtensions
{
    // Every controller that needs "the current user" calls this instead of
    // reading claims directly. Centralizing it means there's exactly one
    // place that knows which claim holds the user id.
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Token is missing a user id.");
        return Guid.Parse(value);
    }
}
