namespace Masa.Utils.DynamicsCrm.Core.Domain.Entities.Auditing;

public interface ICrmModificationAudited : ICrmHasModificationTime
{
    Guid? ModifiedBy { get; set; }
}
