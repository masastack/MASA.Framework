namespace Masa.Utils.DynamicsCrm.Core.Domain.Entities.Auditing;

public interface ICrmHasModificationTime
{
    DateTime? ModifiedOn { get; set; }
}
