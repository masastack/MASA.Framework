namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

public record ForgotPasswordEvent() : Event
{
    public string Account { get; set; }

    public string Email { get; set; }
}
