namespace Masa.Utils.DynamicsCrm.Core.Domain.Entities.Auditing;

public class CrmFullAuditEntity : CrmAuditEntity, ICrmDeletionAudited
{
    public virtual int StateCode { set; get; }
    public virtual int? StatusCode { set; get; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public virtual byte[]? VersionNumber { set; get; }
}
