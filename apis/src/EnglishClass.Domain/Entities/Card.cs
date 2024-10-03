using EnglishClass.Domain.Entities.Auditing;

namespace EnglishClass.Domain.Entities;

public class Card : FullAuditedEntity<Guid>
{
    public required string Text { get; set; }
    public required string Description { get; set; }
}
