namespace Masa.Contrib.Configuration;

public class DefaultMasaConfiguration : IMasaConfiguration
{
    private readonly IConfiguration _configuration;

    public DefaultMasaConfiguration(IConfiguration configuration) => _configuration = configuration;

    public IConfiguration GetConfiguration(SectionTypes sectionType) => _configuration.GetSection(sectionType.ToString());

    /// <summary>
    /// Get the value of the specified key from the DataDictionary of ConfigurationAPI
    /// </summary>
    /// <param name="sectionName">Node name, sectionName is appid if using the capability of Masa.Contrib.BasicAbility.Dcc</param>
    /// <param name="key"></param>
    /// <returns></returns>
    public string? GetValueByDataDictionary(string sectionName, string key)
    {
        List<string> sections = new()
        {
            SectionTypes.ConfigurationAPI.ToString()
        };
        if(!string.IsNullOrWhiteSpace(sectionName))
            sections.Add(sectionName);
        sections.Add(key);
        var path = string.Join(ConfigurationPath.KeyDelimiter,sections);
        return _configuration.GetSection(path)?.ToString();
    }
}
