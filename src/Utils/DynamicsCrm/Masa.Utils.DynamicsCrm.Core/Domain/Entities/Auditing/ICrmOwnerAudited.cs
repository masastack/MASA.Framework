namespace Masa.Utils.DynamicsCrm.Core.Domain.Entities.Auditing;

public interface ICrmOwnerAudited
{
    Guid OwnerId { get; set; }
    int OwnerIdType { get; set; }
    Guid? OwningBusinessUnit { get; set; }
}
