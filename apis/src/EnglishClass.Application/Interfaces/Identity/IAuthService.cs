using System.Security.Claims;
using EnglishClass.Application.Models;
using EnglishClass.Common.Dependencies;

namespace EnglishClass.Application.Interfaces.Identity;

public interface IAuthService : ITransientDependency
{
    Task<bool> SignInAsync(SignInRequest signInRequest);
    Task SignOutAsync();
    Task<Response> SignUpAsync(SignUpRequest signUpRequest);
    Task<UserDto?> GetCurrentUserAsync(ClaimsPrincipal user);
}
