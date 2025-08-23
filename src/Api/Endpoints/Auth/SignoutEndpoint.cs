using FastEndpoints;
using Application.Contracts.Identity;
using Application.Contracts.Application;

namespace Api.Endpoints.Auth;

public class SignoutEndpoint(IAuthService authService, ICurrentUser currentUser) : Ep
    .NoReq
    .NoRes
{
    public override void Configure()
    {
        Post("signout");
        Group<AuthGroup>();
        Description(bld => bld.WithName("Auth_Signout")
            .Produces(statusCode: StatusCodes.Status202Accepted)
            .Produces(statusCode: StatusCodes.Status401Unauthorized));
        Summary(s =>
        {
            s.Summary = "Sign out a user.";
            s.Description = "An async endpoint to sign out the current authenticated user and revoke their refresh tokens.";
        });
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || string.IsNullOrEmpty(currentUser.UserId))
        {
            ThrowError("User not authenticated", StatusCodes.Status401Unauthorized);
            return;
        }

        await authService.SignOutAsync(currentUser.UserId);
        await Send.NoContentAsync(cancellationToken);
    }
}
