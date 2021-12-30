namespace MASA.Contrib.Dispatcher.Events.BenchmarkDotnet.Tests.Extensions.Events;

public record RegisterUserEvent : Event
{
    public string Name { get; set; }

    public string PhoneNumber { get; set; }
}
