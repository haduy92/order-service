using FastEndpoints;
using FluentValidation;
using Application.Contracts.Identity;
using Application.Models.Auth;

namespace Api.Endpoints.Auth;

public class SignupEndpoint(IAuthService authService) : Ep
    .Req<SignupEndpoint.RequestDto>
    .Res<IdentityResponse>
{
    public override void Configure()
    {
        Post("signup");
        Group<AuthGroup>();
        AllowAnonymous();
        Description(bld => bld.WithName("Auth_Signup")
            .Produces<IdentityResponse>(statusCode: StatusCodes.Status200OK)
            .Produces(statusCode: StatusCodes.Status400BadRequest)
            .Produces(statusCode: StatusCodes.Status409Conflict));
        Summary(s =>
        {
            s.Summary = "Sign up a new user.";
            s.Description = "An async endpoint to register a new user account.";
            s.ExampleRequest = new RequestDto
            {
                Email = "user@example.com",
                Password = "SecurePassword123!"
            };
        });
    }

    public override async Task<IdentityResponse> ExecuteAsync(RequestDto req, CancellationToken cancellationToken)
    {
        return await authService.SignUpAsync(req.Email, req.Password);
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
