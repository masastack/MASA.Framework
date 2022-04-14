namespace Masa.Contrib.Configuration.Tests.Config;

public class RabbitMqOptions : LocalMasaConfigurationOptions
{
    public string HostName { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string VirtualHost { get; set; }

    public string Port { get; set; }

    /// <summary>
    /// The section null means same as the class name, else load from the specify section
    /// </summary>
    [JsonIgnore]
    public override string? Section => "RabbitMq";
}
