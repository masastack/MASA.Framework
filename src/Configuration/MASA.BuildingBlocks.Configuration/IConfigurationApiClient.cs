namespace MASA.BuildingBlocks.Configuration;
public interface IConfigurationApiClient
{
    Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawAsync(string environment, string cluster, string appId, string configObject, Action<string> valueChanged);

    Task<T> GetAsync<T>(string environment, string cluster, string appId, string configObject, Action<T> valueChanged);

    Task<dynamic> GetDynamicAsync(string environment, string cluster, string appId, string configObject, Action<dynamic> valueChanged);

    Task<dynamic> GetDynamicAsync(string key);
}

