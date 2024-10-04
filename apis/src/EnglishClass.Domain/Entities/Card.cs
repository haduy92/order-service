using EnglishClass.Domain.Entities.Auditing;

namespace EnglishClass.Domain.Entities;

public class Card : FullAuditedEntity<int>
{
    public required string Text { get; set; }
    public required string Description { get; set; }
}
