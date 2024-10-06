using FlashCard.Domain.Entities.Auditing;

namespace FlashCard.Domain.Entities;

public class Card : FullAuditedEntity<int>
{
    public required string Title { get; set; }
    public required string Description { get; set; }
}
