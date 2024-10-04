using System.Security.Claims;
using AutoMapper;
using EnglishClass.Application.Interfaces;
using EnglishClass.Application.Models;
using EnglishClass.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;

namespace EnglishClass.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IMapper _mapper;

    public AuthService(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    public async Task<bool> SignInAsync(SignInRequest request)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, false);
        return signInResult.Succeeded;
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<Response> SignUpAsync(SignUpRequest request)
    {
        var user = new IdentityUser
        {
            Email = request.Email,
            UserName = request.Email
        };

        IdentityResult result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
        }

        return result.ToAuthenticationResult();
    }

    public async Task<UserDto?> GetCurrentUserAsync(ClaimsPrincipal principal)
    {
        if (principal == null)
        {
            return null;
        }

        var userId = _userManager.GetUserId(principal);
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return null;
        }

        return _mapper.Map<UserDto>(user);
    }
}
