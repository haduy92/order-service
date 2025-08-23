using FastEndpoints;
using FluentValidation;
using Application.Contracts.Identity;
using Application.Models.Auth;

namespace Api.Endpoints.Auth;

public class RefreshTokenEndpoint(IAuthService authService) : Ep
    .Req<RefreshTokenEndpoint.RequestDto>
    .Res<IdentityResponse>
{
    public override void Configure()
    {
        Post("refresh-token");
        Group<AuthGroup>();
        AllowAnonymous();
        Description(bld => bld.WithName("Auth_RefreshToken")
            .Produces<IdentityResponse>(statusCode: StatusCodes.Status200OK)
            .Produces(statusCode: StatusCodes.Status400BadRequest)
            .Produces(statusCode: StatusCodes.Status401Unauthorized));
        Summary(s =>
        {
            s.Summary = "Refresh access token.";
            s.Description = "An async endpoint to refresh an expired access token using a valid refresh token.";
            s.ExampleRequest = new RequestDto
            {
                RefreshToken = "refresh_token_example_12345"
            };
            s.ResponseExamples[200] = new IdentityResponse
            {
                UserId = "12345",
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                RefreshToken = "new_refresh_token_example"
            };
        });
    }

    public override async Task<IdentityResponse> ExecuteAsync(RequestDto req, CancellationToken cancellationToken)
    {
        try
        {
            return await authService.RefreshTokenAsync(req.RefreshToken);
        }
        catch (UnauthorizedAccessException ex)
        {
            ThrowError(ex.Message, StatusCodes.Status401Unauthorized);
            return null!; // This will never execute
        }
    }

    public sealed record RequestDto
    {
        public required string RefreshToken { get; init; }
    }

    public class RequestValidator : Validator<RequestDto>
    {
        public RequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token is required.");
        }
    }
}
