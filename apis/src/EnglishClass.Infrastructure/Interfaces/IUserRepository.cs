using EnglishClass.Domain.Entities;

namespace EnglishClass.Infrastructure.Interfaces;

public interface IUserRepository : IRepository<User, Guid>
{ }
