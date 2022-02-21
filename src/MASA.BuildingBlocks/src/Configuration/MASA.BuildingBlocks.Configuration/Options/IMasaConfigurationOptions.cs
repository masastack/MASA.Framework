namespace MASA.BuildingBlocks.Configuration.Options;
/// <summary>
/// Automatic mapping relationship specification. 
/// When RootSection is Null or an empty string, the configuration will be mounted to the root node. 
/// When Section is Null, the configuration will be mounted under the RootSection node, and its node name is class name.
/// If Section is an empty string, it will be directly mounted under the RootSection node
/// </summary>
public interface IMasaConfigurationOptions
{
    [JsonIgnore]
    string? ParentSection { get; init; }

    /// <summary>
    /// Node name, adjust the mapping relationship by modifying the node name
    /// </summary>
    [JsonIgnore]
    string? Section { get; init; }

    [JsonIgnore]
    SectionTypes SectionType { get; init; }
}
