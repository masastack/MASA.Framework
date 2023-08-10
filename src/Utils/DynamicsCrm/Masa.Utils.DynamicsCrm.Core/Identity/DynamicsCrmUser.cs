namespace Masa.Utils.DynamicsCrm.Core.Identity;

public class DynamicsCrmUser : IIdentityUser
{
    public string Id { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();

    public Guid BusinessUnitId { get; set; }

    public string GetUserName()
    {
        return UserName ?? string.Empty;
    }
}
