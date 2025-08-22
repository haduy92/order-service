using FastEndpoints;

namespace Api.Endpoints.Auth;

public class AuthGroup : Group
{
    public AuthGroup()
    {
        Configure("/v1/auth/", ep =>
        {
            const string tag = "Authentication";

            ep.Options(x =>
            {
                x.WithTags([tag]);
                x.WithSummary("Authentication Operations");
                x.WithDescription("Endpoints for user authentication, registration, and token management");
            });
        });
    }
}
