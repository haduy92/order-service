using FastEndpoints;
using FluentValidation;
using Application.Interfaces.Identity;
using Application.Models.Auth;

namespace Api.Endpoints.Auth;

public class SigninEndpoint(IAuthService authService) : Ep
    .Req<SigninEndpoint.RequestDto>
    .Res<IdentityResponse>
{
    public override void Configure()
    {
        Post("signin");
        Group<AuthGroup>();
        AllowAnonymous();
        Description(bld => bld.WithName("Auth_Signin")
            .Produces<IdentityResponse>(statusCode: StatusCodes.Status200OK)
            .Produces(statusCode: StatusCodes.Status400BadRequest)
            .Produces(statusCode: StatusCodes.Status401Unauthorized));
        Summary(s =>
        {
            s.Summary = "Sign in a user.";
            s.Description = "An async endpoint to authenticate a user and return access tokens.";
            s.ExampleRequest = new RequestDto
            {
                Email = "user@example.com",
                Password = "SecurePassword123!"
            };
            s.ResponseExamples[200] = new IdentityResponse
            {
                UserId = "12345",
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                RefreshToken = "refresh_token_example"
            };
        });
    }

    public override async Task<IdentityResponse> ExecuteAsync(RequestDto req, CancellationToken cancellationToken)
    {
        try
        {
            return await authService.SignInAsync(req.Email, req.Password);
        }
        catch (UnauthorizedAccessException ex)
        {
            ThrowError(ex.Message, StatusCodes.Status401Unauthorized);
            return null!; // This will never execute
        }
    }

    public sealed record RequestDto
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }

    public class RequestValidator : Validator<RequestDto>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long.");
        }
    }
}
