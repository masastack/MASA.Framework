namespace Masa.Contrib.Ddd.Domain.Tests.Events;

public record ProductItemDomainQuery : DomainQuery<string>
{
    public string ProductId { get; set; }

    public override string Result { get; set; }
}
