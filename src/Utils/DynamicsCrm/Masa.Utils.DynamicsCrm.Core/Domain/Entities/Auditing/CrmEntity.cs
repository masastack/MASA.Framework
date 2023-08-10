namespace Masa.Utils.DynamicsCrm.Core.Domain.Entities.Auditing;

public class CrmEntity : Entity<Guid>, ICrmEntity
{
    public virtual Guid? CreatedBy { set; get; }

    public virtual DateTime? CreatedOn { set; get; }

    public virtual Guid? ModifiedBy { set; get; }

    public virtual DateTime? ModifiedOn { set; get; }
}
