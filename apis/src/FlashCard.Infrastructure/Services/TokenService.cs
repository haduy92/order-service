using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FlashCard.Application.Interfaces.Identity;
using FlashCard.Application.Models;
using FlashCard.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FlashCard.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;

    public TokenService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateAccessToken(string userId, string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
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
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            SigningCredentials = credentials
        };

        return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }

    public RefreshTokenDto GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return new()
        {
            RefreshToken = Convert.ToBase64String(randomNumber),
            Expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays)
        };
    }
}
