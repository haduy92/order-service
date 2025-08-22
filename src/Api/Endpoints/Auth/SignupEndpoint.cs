using FastEndpoints;
using FluentValidation;
using Application.Interfaces.Identity;
using Application.Models.Auth;

namespace Api.Endpoints.Auth;

public class SignupEndpoint(IAuthService authService) : Ep
    .Req<SignupEndpoint.RequestDto>
    .NoRes
{
    public override void Configure()
    {
        Post("signup");
        Group<AuthGroup>();
        AllowAnonymous();
        Description(bld => bld.WithName("Auth_Signup")
            .Produces(statusCode: StatusCodes.Status200OK)
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

    public override async Task HandleAsync(RequestDto req, CancellationToken cancellationToken)
    {
        var signUpRequest = req.ToSignUpRequest();
        var result = await authService.SignUpAsync(signUpRequest);

        if (result.Succeeded)
        {
            await Send.OkAsync(cancellationToken);
        }
        else
        {
            // Handle error cases
            if (result.Errors?.Any(e => e.Contains("email_already_existed")) == true)
            {
                ThrowError("Email already exists", StatusCodes.Status409Conflict);
            }
            else
            {
                ThrowError(string.Join(", ", result.Errors ?? ["Registration failed"]), StatusCodes.Status400BadRequest);
            }
        }
    }

    public sealed record RequestDto
    {
        public required string Email { get; init; }
        public required string Password { get; init; }

        public SignUpRequest ToSignUpRequest()
        {
            return new SignUpRequest
            {
                Email = Email,
                Password = Password
            };
        }
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
