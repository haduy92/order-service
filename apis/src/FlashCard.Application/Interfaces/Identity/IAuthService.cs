using FlashCard.Application.Models;

namespace FlashCard.Application.Interfaces.Identity;

public interface IAuthService
{
    Task<IdentityResponse> SignInAsync(SignInRequest signInRequest);
    Task<IdentityResponse> SignUpAsync(SignUpRequest signUpRequest);
    Task<IdentityResponse> RefreshTokenAsync(string refreshToken);
    Task SignOutAsync(string userId);
    Task<UserDto?> GetProfileAsync(string userId);
}
