namespace MASA.Contrib.Dispatcher.IntegrationEvents.Tests.Events;

public class ForgetPasswordEvent : IntegrationEvent
{
    public override string Topic { get; set; } = nameof(ForgetPasswordEvent);

    public string Account { get; set;  }
}
