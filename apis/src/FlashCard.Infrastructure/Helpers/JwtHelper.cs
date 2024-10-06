using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FlashCard.Infrastructure.Helpers;

public static class JwtHelper
{
    /// <summary>
    /// Generates an encoded JWT (JSON Web Token) for a user.
    /// </summary>
    public static string GenerateEncodedToken(string userId, DateTime expire, string issuer, string audience, string key, IEnumerable<string>? roles = null)
    {
        // Initialize a list of claims for the JWT. These include the user's ID and device information,
        // a unique identifier for the JWT, and the time the token was issued.
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.Ticks.ToString(), ClaimValueTypes.Integer64),
        };

        // If any roles are provided, add them to the list of claims. Each role is a separate claim.
        if (roles?.Any() == true)
        {
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        // Create the JWT security token and encode it.
        // The JWT includes the claims defined above, the issuer and audience from the config, and an expiration time.
        // It's signed with a symmetric key, also from the config, and the HMAC-SHA256 algorithm.
        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expire,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256)
        );

        // Convert the JWT into a string format that can be included in an HTTP header.
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
