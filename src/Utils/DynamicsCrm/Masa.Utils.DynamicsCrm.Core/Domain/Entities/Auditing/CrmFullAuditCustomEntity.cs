namespace Masa.Utils.DynamicsCrm.Core.Domain.Entities.Auditing;

public class CrmFullAuditCustomEntity : CrmFullAuditEntity, ICrmCustomEntity
{
    [Column("new_name")]
    public virtual string? Name { set; get; }
}
