using System.Security.Claims;
using FlashCard.Application.Models;

namespace FlashCard.Application.Interfaces.Identity;

public interface IAuthService
{
    Task<IdentityResponse> SignInAsync(SignInRequest signInRequest);
    Task<IdentityResponse> SignUpAsync(SignUpRequest signUpRequest);
    Task SignOutAsync();
    Task<UserDto?> GetCurrentUserAsync(ClaimsPrincipal user);
}
