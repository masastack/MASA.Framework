namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests.Events;

public record RegisterUserIntegrationEvent : IntegrationEvent
{
    public RegisterUserIntegrationEvent()
    {

    }

    public RegisterUserIntegrationEvent(Guid id, DateTime creationTime) : base(id, creationTime)
    {

    }

    public string Account { get; set; }

    public string Password { get; set; }

    public override string Topic { get; set; } = nameof(RegisterUserIntegrationEvent);
}
