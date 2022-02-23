namespace MASA.Contrib.BasicAbility.Dcc.Internal;

public class ConfigurationAPIBase
{
    private readonly DccSectionOptions _defaultSectionOption;
    private readonly List<DccSectionOptions> _expandSectionOptions;

    protected ConfigurationAPIBase(DccSectionOptions defaultSectionOption, List<DccSectionOptions>? expandSectionOptions)
    {
        _defaultSectionOption = defaultSectionOption;
        _expandSectionOptions = expandSectionOptions ?? new();
    }

    protected string GetSecret(string appId)
    {
        if (_defaultSectionOption.AppId == GetAppId(appId))
            return _defaultSectionOption.Secret ?? "";

        var option = _expandSectionOptions.FirstOrDefault(x => x.AppId == appId);
        if (option == null)
            throw new ArgumentNullException(nameof(appId));

        return option.Secret ?? "";
    }

    protected string GetEnvironment(string environment)
        => !string.IsNullOrEmpty(environment) ? environment : _defaultSectionOption.Environment!;

    protected string GetCluster(string cluster)
        => !string.IsNullOrEmpty(cluster) ? cluster : _defaultSectionOption.Cluster!;

    protected string GetAppId(string appId)
        => !string.IsNullOrEmpty(appId) ? appId : _defaultSectionOption.AppId!;

    protected string GetConfigObject(string configObject)
        => !string.IsNullOrEmpty(configObject) ? configObject : throw new ArgumentNullException(nameof(configObject));
}
