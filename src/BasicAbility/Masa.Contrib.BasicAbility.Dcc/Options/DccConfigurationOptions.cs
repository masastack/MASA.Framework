namespace Masa.Contrib.BasicAbility.Dcc.Options;

public class DccConfigurationOptions
{
    public RedisConfigurationOptions RedisOptions { get; set; }

    public string ManageServiceAddress { get; set; } = default!;

    /// <summary>
    /// The prefix of Dcc PubSub, it is not recommended to modify
    /// </summary>
    public string? SubscribeKeyPrefix { get; set; }
}
