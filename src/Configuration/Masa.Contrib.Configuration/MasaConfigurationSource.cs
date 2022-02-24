namespace Masa.Contrib.Configuration;

public class MasaConfigurationSource : IConfigurationSource
{
    internal MasaConfigurationBuilder Builder;

    public MasaConfigurationSource(MasaConfigurationBuilder builder)
    {
        Builder = builder;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new MasaConfigurationProvider(this);
    }
}

