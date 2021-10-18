namespace MASA.Contrib.DDD.Domain.Tests.Events;

public record ProductItemDomainQuery : DomainQuery<string>
{
    public string ProductId { get; set; }

    public override string Result { get; set; }
}
