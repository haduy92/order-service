using EnglishClass.Application.Models;
using Microsoft.AspNetCore.Identity;

namespace EnglishClass.Infrastructure.Extensions;

public static class IdentityResultExtensions
{
    public static Response ToAuthenticationResult(this IdentityResult result)
    {
        return new()
        {
            Succeeded = result.Succeeded,
            Errors = result.Errors.ToDictionary(e => e.Code, e => e.Description)
        };
    }
}
