using System.Security.Claims;
using AutoMapper;
using FlashCard.Application.Interfaces.Identity;
using FlashCard.Application.Models;
using FlashCard.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using FlashCard.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using FlashCard.Infrastructure.Helpers;

namespace FlashCard.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;
    private readonly JwtOptions _jwtSettings;

    public AuthService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IMapper mapper,
        IOptions<JwtOptions> jwtSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<IdentityResponse> SignInAsync(SignInRequest request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, false);

        if (result.Succeeded)
        {
            // If login was successful, get the user and their roles.
            var user = await _userManager.FindByNameAsync(request.Email);
            var userId = user!.Id;
            var userRoles = await _userManager.GetRolesAsync(user);

            // Sign out the user to clear any existing sessions.
            await _signInManager.SignOutAsync();

            // Generate a new token for the user.
            var token = GenerateToken(userId, userRoles);

            return new()
            {
                Succeeded = true,
                UserId = userId,
                Token = token,
                Expire = _jwtSettings.ExpirationTime
            };
        }

        return ErrorResponse();
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<IdentityResponse> SignUpAsync(SignUpRequest request)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            // If signup was successful, get the user and their roles.
            var createdUser = await _userManager.FindByNameAsync(user.UserName);
            var userId = createdUser!.Id;
            var userRoles = await _userManager.GetRolesAsync(createdUser);

            // Generate a new token for the user.
            var token = GenerateToken(userId, userRoles);

            await _signInManager.SignInAsync(user, isPersistent: false);

            return new()
            {
                Succeeded = true,
                UserId = userId,
                Token = token,
                Expire = _jwtSettings.ExpirationTime
            };
        }

        return ErrorResponse(result.Errors.ToDictionary(x => x.Code, x => x.Description));
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

    private string GenerateToken(string userId, IEnumerable<string>? roles = null)
    {
        return JwtHelper.GenerateEncodedToken(userId,
            _jwtSettings.ExpirationTime,
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            _jwtSettings.Key,
            roles);
    }

    private IdentityResponse ErrorResponse(IDictionary<string, string>? errors = null)
    {
        return new()
        {
            Succeeded = false,
            UserId = string.Empty,
            Token = string.Empty,
            Expire = DateTime.UtcNow,
            Errors = errors
        };
    }
}
