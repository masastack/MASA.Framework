namespace MASA.Contrib.Dispatcher.InMemory.Tests.Events;

public class ForgotPasswordEvent : Event
{
    public string Account { get; set; }

    public string Email { get; set; }
}