namespace MASA.Contrib.DDD.Domain.Tests.Events;

public record CreateProductDomainCommand : DomainCommand
{
    public string Name { get; set; }
}
