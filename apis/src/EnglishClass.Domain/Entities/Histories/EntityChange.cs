namespace EnglishClass.Domain.Entities.Histories;

public class EntityChange : Entity<long>
{
    /// <summary>
    /// ChangeTime.
    /// </summary>
    public virtual DateTime ChangeTime { get; set; }

    /// <summary>
    /// ChangeType.
    /// </summary>
    public virtual EntityChangeType ChangeType { get; set; }

    /// <summary>
    /// Gets/sets change set id, used to group entity changes.
    /// </summary>
    public virtual long EntityChangeSetId { get; set; }

    /// <summary>
    /// Gets/sets primary key of the entity.
    /// </summary>
    public virtual string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// FullName of the entity type.
    /// </summary>
    public virtual string EntityTypeFullName { get; set; } = string.Empty;

    /// <summary>
    /// PropertyChanges.
    /// </summary>
    public virtual ICollection<EntityPropertyChange> PropertyChanges { get; set; } = default!;
}
