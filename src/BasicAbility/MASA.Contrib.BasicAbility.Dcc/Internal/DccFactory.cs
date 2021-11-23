namespace MASA.Contrib.BasicAbility.Dcc.Internal;

internal class DccFactory
{
    public static IConfigurationAPIClient CreateClient(
        IServiceProvider serviceProvider,
        IMemoryCacheClient client,
        JsonSerializerOptions jsonSerializerOptions,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
    {
        return new ConfigurationAPIClient(serviceProvider, client, jsonSerializerOptions, defaultSectionOption, expandSectionOptions);
    }

    public static IConfigurationAPIManage CreateManage(
        ICallerFactory callerFactory,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
        => new ConfigurationAPIManage(callerFactory.CreateClient(), defaultSectionOption, expandSectionOptions);
}
