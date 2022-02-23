namespace MASA.Contrib.Configuration.ErrorSectionAutoMap.Tests;

public class ErrorKafkaOptions : KafkaOptions
{
    [JsonIgnore]
    public override string? ParentSection { get; init; } = "Appsettings";

    public ErrorKafkaOptions()
    {
        base.Section = "KafkaOptions";
    }
}
