using Application.Models.Auth;

namespace Application.Interfaces.Identity;

public interface IAuthService
{
    Task<IdentityResponse> SignInAsync(string email, string password);
    Task<IdentityResponse> SignUpAsync(string email, string password);
    Task<IdentityResponse> RefreshTokenAsync(string refreshToken);
    Task SignOutAsync(string userId);
    Task<UserDto?> GetProfileAsync(string userId);
}

