namespace Masa.Contrib.BasicAbility.Dcc.Internal.Options;

internal class DccOptions
{
    public DccConfigurationOptions DccConfigurationOptions { get; }

    public DccSectionOptions DefaultSectionOptions { get; }

    public List<DccSectionOptions> ExpansionSectionOptions { get; }

    public DccOptions(DccConfigurationOptions dccConfigurationOptions, DccSectionOptions defaultSectionOptions, List<DccSectionOptions> expansionSectionOptions)
    {
        DccConfigurationOptions = dccConfigurationOptions;
        DefaultSectionOptions = defaultSectionOptions;
        ExpansionSectionOptions = expansionSectionOptions;
    }
}
