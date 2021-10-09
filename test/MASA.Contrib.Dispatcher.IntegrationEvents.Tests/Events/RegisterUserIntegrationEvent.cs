namespace MASA.Contrib.Dispatcher.IntegrationEvents.Tests.Events;

public class RegisterUserIntegrationEvent : IntegrationEvent
{
    public RegisterUserIntegrationEvent()
    {
        Topic = typeof(RegisterUserIntegrationEvent).Name;
    }

    public string Account { get; set; }

    public string Password { get; set; }

    public override string Topic { get; set; }
}
