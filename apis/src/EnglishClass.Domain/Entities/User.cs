using EnglishClass.Domain.Entities.Auditing;

namespace EnglishClass.Domain.Entities;

public class User : FullAuditedEntity<Guid>
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required Address Address { get; set; }
    public required EmailAddress Email { get; set; }
    public required PhoneNumber MobilePhone { get; set; }
}
