namespace MASA.Contrib.DDD.Domain;

public class DomainService : IDomainService
{
    public IDomainEventBus EventBus { get; }

    public DomainService(IDomainEventBus eventBus) => EventBus = eventBus;
}
