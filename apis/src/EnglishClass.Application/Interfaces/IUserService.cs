using EnglishClass.Application.Models;
using EnglishClass.Common.Dependencies;

namespace EnglishClass.Application.Interfaces;

public interface IUserService : ITransientDependency
{
    Task<Guid> Create(UserModel user);
}
