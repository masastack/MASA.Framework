namespace MASA.Contrib.DDD.Domain.Tests.Services;

public class UserDomainService : DomainService
{
    public UserDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    public async Task<string> RegisterUserSucceededAsync(RegisterUserSucceededDomainIntegrationEvent domainIntegrationEvent)
    {
        // TODO Simulate a successful message for registered users

        await EventBus.PublishAsync(domainIntegrationEvent);
        return "succeed";
    }
}
