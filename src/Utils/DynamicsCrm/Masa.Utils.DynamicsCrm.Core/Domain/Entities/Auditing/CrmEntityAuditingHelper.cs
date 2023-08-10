namespace Masa.Utils.DynamicsCrm.Core.Domain.Entities.Auditing;

public static class CrmEntityAuditingHelper
{
    public static void SetCreationAuditProperties(
        object entityAsObj,
        Guid systemUserId,
        Guid ownerId,
        Guid businessUnitId)
    {
        var entityWithCrmCreatedAudit = entityAsObj as ICrmCreationAudited;
        if (entityWithCrmCreatedAudit == null)
        {
            return;
        }

        if (entityWithCrmCreatedAudit.CreatedOn == default(DateTime) || !entityWithCrmCreatedAudit.CreatedOn.HasValue)
        {
            entityWithCrmCreatedAudit.CreatedOn = DateTime.UtcNow;
        }
        entityWithCrmCreatedAudit.CreatedBy = systemUserId;
        if (entityAsObj is ICrmOwnerAudited ownerAudited)
        {
            if (ownerAudited.OwnerId == Guid.Empty)
            {
                ownerAudited.OwnerId = ownerId;
            }
            ownerAudited.OwnerIdType = 8;
            if (ownerAudited.OwningBusinessUnit == null)
            {
                ownerAudited.OwningBusinessUnit = businessUnitId;
            }
        }
        if (entityAsObj is ICrmDeletionAudited deletionAudited)
        {
            deletionAudited.StateCode = 0;
            deletionAudited.StatusCode = 1;
        }

        SetModificationAuditProperties(entityAsObj, systemUserId);
    }

    public static void SetModificationAuditProperties(
        object entityAsObj,
        Guid systemUserId)
    {
        var entityWithCrmModifiedAudit = entityAsObj as ICrmModificationAudited;
        if (entityWithCrmModifiedAudit == null)
        {
            return;
        }

        entityWithCrmModifiedAudit.ModifiedOn = DateTime.UtcNow;
        entityWithCrmModifiedAudit.ModifiedBy = systemUserId;
    }

    public static void SetDeletionAuditProperties(
        object entityAsObj,
        Guid systemUserId)
    {
        var entityWithCrmDeletionAudited = entityAsObj as ICrmDeletionAudited;
        if (entityWithCrmDeletionAudited == null)
        {
            return;
        }

        entityWithCrmDeletionAudited.StateCode = 1;
        entityWithCrmDeletionAudited.StatusCode = 2;
        entityWithCrmDeletionAudited.ModifiedOn = DateTime.UtcNow;
        entityWithCrmDeletionAudited.ModifiedBy = systemUserId;
    }
}
