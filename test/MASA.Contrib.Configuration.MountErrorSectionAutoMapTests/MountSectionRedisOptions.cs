namespace MASA.Contrib.Configuration.MountErrorSectionAutoMapTests;

public class MountSectionRedisOptions : MasaConfigurationOptions
{
    [JsonIgnore]
    public override string? ParentSection { get; init; } = "Appsettings";

    [JsonIgnore]
    public override string? Section { get; init; } = null;

    public override SectionTypes SectionType { get; init; } = SectionTypes.ConfigurationAPI;
}
