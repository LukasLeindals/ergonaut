using System.Security.Claims;

namespace Ergonaut.App.Auth;

public interface ITokenService
{
    string CreateAccessToken(IEnumerable<Claim> claims, int? ttlMinutes = null);
}
