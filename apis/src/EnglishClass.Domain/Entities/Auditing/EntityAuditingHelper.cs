using Common.Extensions;
using Common.Timing;

namespace EnglishClass.Domain.Entities.Auditing
{
    public static class EntityAuditingHelper
    {
        public static void SetCreationAuditProperties(object entityAsObj, string userId)
        {
            var entityWithCreationTime = entityAsObj as IHasCreationTime;
            if (entityWithCreationTime is null)
            {
                //Object does not implement IHasCreationTime
                return;
            }

            if (entityWithCreationTime.CreationTime == default)
            {
                entityWithCreationTime.CreationTime = Clock.Now;
            }

            if (entityAsObj is not ICreationAudited)
            {
                //Object does not implement ICreationAudited
                return;
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                //Unknown user
                return;
            }

            var entity = entityAsObj as ICreationAudited;
            if (entity is not null && entity.CreatorUserId is not null)
            {
                //CreatorUserId is already set
                return;
            }

            //Finally, set CreatorUserId!
            entity!.CreatorUserId = userId;
        }

        public static void SetModificationAuditProperties(object entityAsObj, string userId)
        {
            if (entityAsObj is IHasModificationTime)
            {
                entityAsObj.As<IHasModificationTime>().LastModificationTime = Clock.Now;
            }

            if (entityAsObj is not IModificationAudited)
            {
                //Entity does not implement IModificationAudited
                return;
            }

            var entity = entityAsObj.As<IModificationAudited>();

            if (string.IsNullOrWhiteSpace(userId))
            {
                //Unknown user
                entity.LastModifierUserId = string.Empty;
                return;
            }

            //Finally, set LastModifierUserId!
            entity.LastModifierUserId = userId;
        }

        public static void SetDeletionAuditProperties(object entityAsObj, string userId)
        {
            if (entityAsObj is IHasDeletionTime)
            {
                entityAsObj.As<IHasDeletionTime>().DeletionTime = Clock.Now;
            }

            if (entityAsObj is not IDeletionAudited)
            {
                //Entity does not implement IDeletionAudited
                return;
            }

            var entity = entityAsObj.As<IDeletionAudited>();

            if (!string.IsNullOrWhiteSpace(entity.DeleterUserId))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                entity.DeleterUserId = string.Empty;
                return;
            }
            //Finally, set LastModifierUserId!
            entity.DeleterUserId = userId;
        }
    }
}