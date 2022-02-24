namespace Masa.Contrib.Ddd.Domain.Tests.Events;

public record CreateProductDomainCommand : DomainCommand
{
    public string Name { get; set; }
}
