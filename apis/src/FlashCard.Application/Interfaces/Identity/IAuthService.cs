using System.Security.Claims;
using FlashCard.Application.Models;

namespace FlashCard.Application.Interfaces.Identity;

public interface IAuthService
{
    Task<bool> SignInAsync(SignInRequest signInRequest);
    Task SignOutAsync();
    Task<Response> SignUpAsync(SignUpRequest signUpRequest);
    Task<UserDto?> GetCurrentUserAsync(ClaimsPrincipal user);
}
