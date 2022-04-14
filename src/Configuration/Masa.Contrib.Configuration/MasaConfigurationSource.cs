namespace Masa.Contrib.Configuration;

public class MasaConfigurationSource : IConfigurationSource
{
    internal readonly MasaConfigurationBuilder Builder;

    public MasaConfigurationSource(MasaConfigurationBuilder builder) => Builder = builder;

    public IConfigurationProvider Build(IConfigurationBuilder builder) => new MasaConfigurationProvider(this);
}
