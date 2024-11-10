using FlashCard.Domain.Entities.Auditing;

namespace FlashCard.Domain.Entities;

public class RefreshToken : CreationAuditedEntity
{
    public required string Token { get; set; }
    public DateTime Expires { get; set; }
    public DateTime? Revoked { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => Revoked == null && !IsExpired;
}
