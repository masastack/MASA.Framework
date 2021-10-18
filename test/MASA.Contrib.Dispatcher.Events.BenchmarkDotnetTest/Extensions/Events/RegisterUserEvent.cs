namespace MASA.Contrib.Dispatcher.Events.BenchmarkDotnetTest.Extensions.Events;

public record RegisterUserEvent : Event
{
    public string Name { get; set; }

    public string Mobile { get; set; }
}
