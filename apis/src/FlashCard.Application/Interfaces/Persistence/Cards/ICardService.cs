using FlashCard.Application.Models;

namespace FlashCard.Application.Interfaces.Persistence.Cards;

public interface ICardService
{
    Task<IEnumerable<CreateCardRequest>> GetByUserId(int id);
    Task<CreateCardRequest> GetById(int id);
    Task<int> Create(CreateCardRequest card);
    Task Update(string text, string description);
    Task Delete(int id);
}
