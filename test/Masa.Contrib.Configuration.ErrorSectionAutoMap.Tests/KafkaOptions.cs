namespace Masa.Contrib.Configuration.ErrorSectionAutoMap.Tests;

public class KafkaOptions : LocalMasaConfigurationOptions
{
    public string Servers { get; set; }

    public int ConnectionPoolSize { get; set; }
}
