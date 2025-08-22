using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Infrastructure")]

namespace Domain.Entities;

/// <summary>
/// A shortcut of <see cref="Entity{TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
/// </summary>
[Serializable]
public abstract class Entity : Entity<int>, IEntity
{

}

/// <summary>
/// Basic implementation of IEntity interface.
/// An entity can inherit this class of directly implement to IEntity interface.
/// </summary>
/// <typeparam name="TPrimaryKey">Type of the primary key of the entity</typeparam>
[Serializable]
public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
{
    /// <summary>
    /// Unique identifier for this entity.
    /// </summary>
    public virtual TPrimaryKey Id { get; set; } = default!;

    /// <summary>
    /// User ID who created this entity. Can only be set by the Infrastructure layer.
    /// </summary>
    public string CreatorUserId { get; internal set; } = string.Empty;
    
    /// <summary>
    /// Date and time when this entity was created. Can only be set by the Infrastructure layer.
    /// </summary>
    public DateTime CreationTime { get; internal set; }
    
    /// <summary>
    /// User ID who last modified this entity. Can only be set by the Infrastructure layer.
    /// </summary>
    public string? LastModifierUserId { get; internal set; }
    
    /// <summary>
    /// Date and time when this entity was last modified. Can only be set by the Infrastructure layer.
    /// </summary>
    public DateTime? LastModificationTime { get; internal set; }
}

