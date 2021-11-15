namespace MASA.Contrib.Configuration.ErrorSectionAutoMapTests;

public class ErrorKafkaOptions : KafkaOptions
{
    [JsonIgnore]
    public override string? ParentSection { get; init; } = "Appsettings";

    public ErrorKafkaOptions()
    {
        base.Section = "KafkaOptions";
    }
}
