using EnglishClass.Domain.Entities.Auditing;

namespace EnglishClass.Domain.Entities.Histories;

public class EntityChangeSet : Entity<long>, IHasCreationTime
{
    /// <summary>
    /// Creation time of this entity.
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// A JSON formatted string to extend the containing object.
    /// </summary>
    public string ExtensionData { get; set; } = string.Empty;

    /// <summary>
    /// UserId.
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// Entity changes grouped in this change set.
    /// </summary>
    public IList<EntityChange> EntityChanges { get; set; } = new List<EntityChange>();
}
