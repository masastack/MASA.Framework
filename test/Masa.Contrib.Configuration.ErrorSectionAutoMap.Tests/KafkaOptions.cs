namespace Masa.Contrib.Configuration.ErrorSectionAutoMap.Tests;

public class KafkaOptions : MasaConfigurationOptions
{
    public string Servers { get; set; }

    public int ConnectionPoolSize { get; set; }

    public override SectionTypes SectionType { get; init; } = SectionTypes.Local;

    public KafkaOptions()
    {
        base.ParentSection = "";
    }
}
