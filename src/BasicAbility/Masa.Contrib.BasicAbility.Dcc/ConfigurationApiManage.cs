namespace Masa.Contrib.BasicAbility.Dcc;

public class ConfigurationApiManage : ConfigurationApiBase, IConfigurationApiManage
{
    private readonly ICallerProvider _callerProvider;

    public ConfigurationApiManage(
        ICallerProvider callerProvider,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
        : base(defaultSectionOption, expandSectionOptions)
    {
        _callerProvider = callerProvider;
    }

    public async Task UpdateAsync(string environment, string cluster, string appId, string configObject, object value)
    {
        var requestUri = $"open-api/releasing/{GetEnvironment(environment)}/{GetCluster(cluster)}/{GetAppId(appId)}/{GetConfigObject(configObject)}?secret={GetSecret(appId)}";
        var result = await _callerProvider.PutAsync(requestUri, value, default);

        // 299 is the status code when throwing a UserFriendlyException in masa.framework
        if ((int)result.StatusCode == 299 || !result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync();
            throw new HttpRequestException(error);
        }
    }
}
