namespace MASA.BuildingBlocks.Configuration.Options;
public class ConfigurationRelationOptions
{
    public SectionTypes SectionType { get; set; }

    public string? ParentSection { get; set; }

    public string Section { get; set; } = default!;

    /// <summary>
    /// Object type of mapping node relationship
    /// </summary>
    public Type ObjectType { get; set; } = default!;
}
