using System.Security.Claims;

namespace Ergonaut.App.Auth;

public interface ITokenService
{
    string CreateAccessToken(IEnumerable<Claim> claims);
    RefreshToken CreateRefreshToken(string subject); // returns token + expiry

}
