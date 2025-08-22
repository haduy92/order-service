using Application.Interfaces.Identity;
using Application.Interfaces.Persistence.Identities;
using Application.Models.Auth;
using Domain.Entities;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Shared.Exceptions;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<IdentityResponse> SignInAsync(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

        if (result.Succeeded)
        {
            // If login was successful, get the user and their roles.
            var user = await _userManager.FindByNameAsync(email);
            var accessToken = _tokenService.GenerateAccessToken(user!.Id, email);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await _refreshTokenRepository.InsertAsync(new RefreshToken
            {
                Token = refreshToken.RefreshToken,
                Expires = refreshToken.Expires,
                CreatorUserId = user.Id
            });

            return new()
            {
                UserId = user!.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken.RefreshToken
            };
        }
        else
        {
            if (result.IsLockedOut)
            {
                throw new UnauthorizedException("Account is locked out");
            }
            if (result.IsNotAllowed)
            {
                throw new UnauthorizedException("Account is not allowed");
            }
            throw new UnauthorizedException("Invalid email or password");
        }
    }

    public async Task<IdentityResponse> SignUpAsync(string email, string password)
    {
        var user = new ApplicationUser
        {
            Email = email,
            UserName = email
        };

        var existingUser = await _userManager.FindByNameAsync(email);
        if (existingUser != null)
        {
            throw new DuplicatedException("Email already exists");
        }

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // If signup was successful, get the user and their roles.
            var createdUser = await _userManager.FindByNameAsync(user.UserName);

            await _signInManager.SignInAsync(user, isPersistent: false);

            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await _refreshTokenRepository.InsertAsync(new RefreshToken { Token = refreshToken.RefreshToken, Expires = refreshToken.Expires, CreatorUserId = user.UserName });

            return new()
            {
                UserId = createdUser!.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken.RefreshToken
            };
        }

        var errors = string.Join(", ", result.Errors.Select(x => $"{x.Code}: {x.Description}"));
        throw new InvalidOperationException($"Registration failed: {errors}");
    }

    public async Task SignOutAsync(string userId)
    {
        await _refreshTokenRepository.RevokeByUserIdAsync(userId);
        await _signInManager.SignOutAsync();
    }

    public async Task<IdentityResponse> RefreshTokenAsync(string refreshToken)
    {
        var existing = await _refreshTokenRepository.GetAsync(refreshToken);
        if (existing is null || !existing.IsActive)
        {
            throw new UnauthorizedException("Invalid or expired refresh token");
        }

        existing.Revoked = DateTime.UtcNow;
        var user = await _userManager.FindByIdAsync(existing.CreatorUserId);
        var accessToken = _tokenService.GenerateAccessToken(user!.Id, user.Email!);
        var refreshTokenDto = _tokenService.GenerateRefreshToken();
        await _refreshTokenRepository.InsertAsync(new RefreshToken
        {
            Token = refreshTokenDto.RefreshToken,
            Expires = refreshTokenDto.Expires,
            CreatorUserId = user.Id
        });

        return new()
        {
            UserId = user!.Id,
            AccessToken = accessToken,
            RefreshToken = refreshTokenDto.RefreshToken
        };
    }

    public async Task<UserDto?> GetProfileAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return null;
        }

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName
        };
    }
}

