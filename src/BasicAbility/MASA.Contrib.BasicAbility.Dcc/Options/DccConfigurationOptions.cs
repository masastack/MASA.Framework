namespace MASA.Contrib.BasicAbility.Dcc.Options;

public class DccConfigurationOptions : RedisConfigurationOptions
{
    public string DccServiceAddress { get; set; } = default!;

    /// <summary>
    /// The prefix of Dcc PubSub, it is not recommended to modify
    /// </summary>
    public string? SubscribeKeyPrefix { get; set; }
}
