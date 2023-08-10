namespace Masa.Utils.DynamicsCrm.Core.Domain.Entities.Auditing;

public class CrmAuditEntity : CrmEntity, ICrmOwnerAudited
{
    public virtual Guid OwnerId { set; get; }

    public virtual int OwnerIdType { set; get; }

    public virtual Guid? OwningBusinessUnit { set; get; }
}

