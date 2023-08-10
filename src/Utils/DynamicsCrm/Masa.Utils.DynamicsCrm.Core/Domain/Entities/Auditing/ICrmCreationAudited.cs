namespace Masa.Utils.DynamicsCrm.Core.Domain.Entities.Auditing;

public interface ICrmCreationAudited : ICrmHasCreationTime
{
    Guid? CreatedBy { get; set; }
}
