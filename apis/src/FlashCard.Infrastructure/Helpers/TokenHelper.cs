using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FlashCard.Infrastructure.Helpers;

public static class TokenHelper
{
    /// <summary>
    /// Generates an encoded JWT (JSON Web Token) for a user.
    /// </summary>
    public static string CreateToken(string userId, string email, string issuer, string audience, string key, DateTime expire)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            // Initialize a list of claims for the JWT. These include the user's ID,
            // a unique identifier for the JWT, and the time the token was issued.
            Subject = new ClaimsIdentity(
            [
                new Claim("userId", userId),
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.Ticks.ToString(), ClaimValueTypes.Integer64)
            ]),
            Issuer = issuer,
            Audience = audience,
            Expires = expire,
            SigningCredentials = credentials
        };

        return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }
}
