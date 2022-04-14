namespace Masa.Contrib.Configuration;

public class DefaultMasaConfiguration : IMasaConfiguration
{
    private readonly IConfiguration _configuration;

    public DefaultMasaConfiguration(IConfiguration configuration) => _configuration = configuration;

    public IConfiguration GetConfiguration(SectionTypes sectionType) => _configuration.GetSection(sectionType.ToString());
}
