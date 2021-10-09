namespace MASA.Contrib.Dispatcher.InMemory.BenchmarkDotnetTest.Extensions.Events;

public class RegisterUserEvent : Event
{
    public string Name { get; set; }

    public string Mobile { get; set; }
}
