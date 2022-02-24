namespace Masa.Contrib.Configuration;

public abstract class MasaConfigurationOptions : IMasaConfigurationOptions
{
    /// <summary>
    /// The name of the parent section, if it is empty, it will be mounted under SectionType, otherwise it will be mounted to the specified section under SectionType
    /// </summary>
    [JsonIgnore]
    public virtual string? ParentSection { get; init; } = null;

    /// <summary>
    /// The section null means same as the class name, else load from the specify section
    /// </summary>
    [JsonIgnore]
    public virtual string? Section { get; init; } = null;

    [JsonIgnore]
    public abstract SectionTypes SectionType { get; init; }
}
