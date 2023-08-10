namespace Masa.Utils.DynamicsCrm.Core.Domain.Entities.Auditing;

public interface ICrmState
{
    int StateCode { get; set; }
    int? StatusCode { get; set; }
}
