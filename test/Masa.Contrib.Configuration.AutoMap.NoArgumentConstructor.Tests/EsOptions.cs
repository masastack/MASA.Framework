namespace Masa.Contrib.Configuration.ErrorSectionAutoMap.NoArgumentConstructor.Tests;

public class EsOptions : LocalMasaConfigurationOptions
{
    public string[] Nodes { get; set; }

    public EsOptions(string[] nodes)
    {
        Nodes = nodes;
    }
}
