namespace MASA.Contrib.ReadWriteSpliting.CQRS.Tests.Commands;

public record CreateProductionCommand : Command
{
    public string Name { get; set; }

    public int Count { get; set; }
}
