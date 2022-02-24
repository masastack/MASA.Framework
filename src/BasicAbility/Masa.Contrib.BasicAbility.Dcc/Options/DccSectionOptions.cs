namespace Masa.Contrib.BasicAbility.Dcc.Options;

public class DccSectionOptions
{
    /// <summary>
    /// The environment name.
    /// Get from the environment variable ASPNETCORE_ENVIRONMENT when Environment is null or empty
    /// </summary>
    public string? Environment { get; set; } = null;

    /// <summary>
    /// The cluster name.
    /// </summary>
    public string? Cluster { get; set; }

    /// <summary>
    /// The app id.
    /// </summary>
    public string AppId { get; set; } = default!;

    public List<string> ConfigObjects { get; set; } = default!;

    public string? Secret { get; set; }
}
