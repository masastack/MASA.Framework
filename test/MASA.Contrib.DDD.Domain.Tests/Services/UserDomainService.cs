namespace MASA.Contrib.DDD.Domain.Tests.Services;

public class UserDomainService : DomainService
{
    public UserDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    public async Task<string> RegisterUserSucceededAsync(string account)
    {
        // TODO Simulate a successful message for registered users

        await EventBus.PublishAsync(new RegisterUserSucceededDomainIntegrationEvent() { Account = account });
        return "succeed";
    }
}
