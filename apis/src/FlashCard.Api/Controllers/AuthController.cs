using Asp.Versioning;
using FlashCard.Application.Interfaces.Identity;
using FlashCard.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlashCard.Api.Controllers;

[ApiExplorerSettings(GroupName = "auth")]
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService,
        ILogger<AuthController> logger)
    {
        _logger = logger;
        _authService = authService;
    }

    /// <summary>
    /// Sign in method
    /// </summary>
    /// <param name="request"></param>
    /// <response code="400">Validation of request failed</response>
    /// <response code="500">Unexpected server error</response>
    /// <returns>True or False</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("signin")]
    public async Task<ActionResult<ResponseBase>> SignInAsync(SignInRequest request)
    {
        try
        {
            // Authenticate user and generate authentication token
            var response = await _authService.SignInAsync(request);

            if (!response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("signup")]
    public async Task<ActionResult<ResponseBase>> SignUpAsync(SignUpRequest request)
    {
        try
        {
            // Register new user
            var response = await _authService.SignUpAsync(request);

            if (response == null || !response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("signout")]
    public async Task<ActionResult> SignOutAsync()
    {
        // Log out user
        await _authService.SignOutAsync();

        return NoContent();
    }
}
