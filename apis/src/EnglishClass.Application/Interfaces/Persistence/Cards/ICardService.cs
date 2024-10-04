using EnglishClass.Application.Models;
using EnglishClass.Common.Dependencies;

namespace EnglishClass.Application.Interfaces.Persistence.Cards;

public interface ICardService : ITransientDependency
{
    Task<IEnumerable<CreateCardRequest>> GetByUserId(int id);
    Task<CreateCardRequest> GetById(int id);
    Task<int> Create(CreateCardRequest card);
    Task Update(string text, string description);
    Task Delete(int id);
}
