using System.Security.Claims;
using FlashCard.Application.Models;
using FlashCard.Common.Dependencies;

namespace FlashCard.Application.Interfaces.Identity;

public interface IAuthService : ITransientDependency
{
    Task<bool> SignInAsync(SignInRequest signInRequest);
    Task SignOutAsync();
    Task<Response> SignUpAsync(SignUpRequest signUpRequest);
    Task<UserDto?> GetCurrentUserAsync(ClaimsPrincipal user);
}
