namespace Masa.Contrib.Configuration;

public class MasaConfigurationBuilder : IMasaConfigurationBuilder
{
    private readonly IConfigurationBuilder _builder;

    public IServiceCollection Services { get; }

    public IDictionary<string, object> Properties => _builder.Properties;

    public IList<IConfigurationSource> Sources => _builder.Sources;

    private IConfiguration? _configuration;

    public IConfiguration Configuration => _configuration ??= _builder.Build();

    internal List<IConfigurationRepository> Repositories { get; } = new();

    internal List<ConfigurationRelationOptions> Relations { get; } = new();

    public MasaConfigurationBuilder(IServiceCollection services, IConfigurationBuilder builder)
    {
        Services = services;
        _builder = builder;
    }

    public void AddRepository(IConfigurationRepository configurationRepository)
        => Repositories.Add(configurationRepository);

    public void AddRelations(params ConfigurationRelationOptions[] relationOptions)
        => Relations.AddRange(relationOptions);

    public IConfigurationBuilder Add(IConfigurationSource source) => _builder.Add(source);

    public IConfigurationRoot Build() => _builder.Build();
}
