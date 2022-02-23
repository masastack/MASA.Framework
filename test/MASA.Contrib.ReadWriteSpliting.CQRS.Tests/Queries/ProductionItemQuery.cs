namespace MASA.Contrib.ReadWriteSpliting.CQRS.Tests.Queries;

public record ProductionItemQuery : Query<string>
{
    public override string Result { get; set; }

    public string ProductionId { get; set; }
}
