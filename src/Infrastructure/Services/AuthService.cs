using AutoMapper;
using Application.Interfaces.Identity;
using Application.Interfaces.Persistence.Identities;
using Application.Models;
using Domain.Entities;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(IMapper mapper,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<IdentityResponse> SignInAsync(SignInRequest request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, false);

        if (result.Succeeded)
        {
            // If login was successful, get the user and their roles.
            var user = await _userManager.FindByNameAsync(request.Email);
            var accessToken = _tokenService.GenerateAccessToken(user!.Id, request.Email);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await _refreshTokenRepository.InsertAsync(new RefreshToken
            {
                Token = refreshToken.RefreshToken,
                Expires = refreshToken.Expires,
                CreatorUserId = user.Id
            });

            return new()
            {
                Succeeded = true,
                UserId = user!.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken.RefreshToken
            };
        }
        else
        {
            if (result.IsLockedOut)
            {
                return ErrorResponse("account_locked_out");
            }
            if (result.IsNotAllowed)
            {
                return ErrorResponse("account_not_allowed");
            }
            return ErrorResponse("invalid_email_or_password");
        }
    }

    public async Task<IdentityResponse> SignUpAsync(SignUpRequest request)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email
        };

        var existingUser = await _userManager.FindByNameAsync(request.Email);
        if (existingUser != null)
        {
            return ErrorResponse("email_already_existed");
        }

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            // If signup was successful, get the user and their roles.
            var createdUser = await _userManager.FindByNameAsync(user.UserName);

            await _signInManager.SignInAsync(user, isPersistent: false);

            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await _refreshTokenRepository.InsertAsync(new RefreshToken { Token = refreshToken.RefreshToken, Expires = refreshToken.Expires });

            return new()
            {
                Succeeded = true,
                UserId = createdUser!.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken.RefreshToken
            };
        }

        return ErrorResponse(result.Errors.Select(x => $"{x.Code}:{x.Description}"));
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
            return ErrorResponse("invalid_refresh_token");
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
            Succeeded = true,
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

        return _mapper.Map<UserDto>(user);
    }

    private IdentityResponse ErrorResponse(IEnumerable<string> errors)
    {
        return new()
        {
            Succeeded = false,
            Errors = errors
        };
    }
    private IdentityResponse ErrorResponse(string error)
    {
        return new()
        {
            Succeeded = false,
            Errors = [error]
        };
    }
}

