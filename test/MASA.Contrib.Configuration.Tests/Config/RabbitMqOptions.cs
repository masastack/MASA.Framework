namespace MASA.Contrib.Configuration.Tests.Config;

public class RabbitMqOptions : MasaConfigurationOptions
{
    public string HostName { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string VirtualHost { get; set; }

    public string Port { get; set; }

    public override SectionTypes SectionType { get; init; } = SectionTypes.Local;
}
