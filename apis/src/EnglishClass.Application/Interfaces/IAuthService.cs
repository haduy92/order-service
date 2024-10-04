using EnglishClass.Common.Dependencies;

namespace EnglishClass.Application.Interfaces;

public interface IAuthService : ITransientDependency
{
    Task<string> Login(string username, string password);
    Task Logout(Guid userId);
}
