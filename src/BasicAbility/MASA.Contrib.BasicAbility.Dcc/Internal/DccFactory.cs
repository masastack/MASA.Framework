namespace MASA.Contrib.BasicAbility.Dcc.Internal;

internal class DccFactory
{
    public static IConfigurationApiClient CreateClient(
        IServiceProvider serviceProvider,
        IMemoryCacheClient client,
        JsonSerializerOptions jsonSerializerOptions,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
    {
        return new ConfigurationApiClient(serviceProvider, client, jsonSerializerOptions, defaultSectionOption, expandSectionOptions);
    }

    public static IConfigurationApiManage CreateManage(
        ICallerFactory callerFactory,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
        => new ConfigurationApiManage(callerFactory.CreateClient(), defaultSectionOption, expandSectionOptions);
}
