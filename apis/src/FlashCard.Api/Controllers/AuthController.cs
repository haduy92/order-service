using Asp.Versioning;
using FlashCard.Application.Interfaces.Application;
using FlashCard.Application.Interfaces.Identity;
using FlashCard.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashCard.Api.Controllers;

[ApiExplorerSettings(GroupName = "auth")]
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService,
        ICurrentUser currentUser,
        ILogger<AuthController> logger)
    {
        _logger = logger;
        _authService = authService;
        _currentUser = currentUser;
    }

    [HttpPost("signin")]
    public async Task<ActionResult<ResponseBase>> SignInAsync(SignInRequest request)
    {
        // Authenticate user and generate authentication token
        var response = await _authService.SignInAsync(request);

        if (!response.Succeeded)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }

    [HttpPost("signup")]
    public async Task<ActionResult<ResponseBase>> SignUpAsync(SignUpRequest request)
    {
        // Register new user
        var response = await _authService.SignUpAsync(request);

        if (response == null || !response.Succeeded)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ResponseBase>> RefreshTokenAsync(string refreshToken)
    {
        var response = await _authService.RefreshTokenAsync(refreshToken);
        return Ok(response);
    }

    [HttpPost("signout")]
    [Authorize]
    public async Task<ActionResult> SignOutAsync()
    {
        // Log out user
        await _authService.SignOutAsync(_currentUser.UserId!);
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult> GetProfileAsync()
    {
        var profile = await _authService.GetProfileAsync(_currentUser.UserId!);
        return Ok(profile);
    }
}
