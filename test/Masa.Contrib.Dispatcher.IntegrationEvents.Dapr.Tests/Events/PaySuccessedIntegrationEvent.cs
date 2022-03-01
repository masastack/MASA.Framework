namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests.Events;

public record RegisterUserIntegrationEvent(Guid Id, DateTime CreationTime) : IntegrationEvent(Id, CreationTime)
{
    public string Account { get; set; }

    public string Password { get; set; }

    public override string Topic { get; set; } = nameof(RegisterUserIntegrationEvent);

    public RegisterUserIntegrationEvent() : this(Guid.NewGuid(), DateTime.UtcNow)
    {

    }
}
