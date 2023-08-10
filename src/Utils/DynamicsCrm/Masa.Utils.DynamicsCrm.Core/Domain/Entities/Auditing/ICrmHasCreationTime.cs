namespace Masa.Utils.DynamicsCrm.Core.Domain.Entities.Auditing;

public interface ICrmHasCreationTime
{
    DateTime? CreatedOn { get; set; }
}
