namespace Domain.Entities;

public class RefreshToken : Entity
{
    public required string Token { get; set; }
    public DateTime Expires { get; set; }
    public DateTime? Revoked { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => Revoked == null && !IsExpired;
}
