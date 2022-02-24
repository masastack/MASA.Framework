namespace Masa.Contrib.ReadWriteSpliting.Cqrs.Tests.Commands;

public record CreateProductionCommand : Command
{
    public string Name { get; set; }

    public int Count { get; set; }
}
