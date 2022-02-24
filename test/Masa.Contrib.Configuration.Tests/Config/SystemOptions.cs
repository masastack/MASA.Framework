namespace Masa.Contrib.Configuration.Tests.Config;

public class SystemOptions : MasaConfigurationOptions
{
    [JsonIgnore]
    public override string? ParentSection { get; init; } = "Appsettings";

    [JsonIgnore]
    public override string? Section { get; init; } = null;

    public override SectionTypes SectionType { get; init; } = SectionTypes.Local;

    public string? Name { get; set; }
}
