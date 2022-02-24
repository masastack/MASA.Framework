namespace Masa.Contrib.ReadWriteSpliting.Cqrs.Tests.Queries;

public record ProductionItemQuery : Query<string>
{
    public override string Result { get; set; }

    public string ProductionId { get; set; }
}
