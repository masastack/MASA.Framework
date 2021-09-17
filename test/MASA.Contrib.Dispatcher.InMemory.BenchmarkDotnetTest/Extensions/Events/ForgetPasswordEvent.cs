namespace MASA.Contrib.Dispatcher.InMemory.BenchmarkDotnetTest.Extensions.Events;

public class ForgetPasswordEvent : Event
{
    public string Name { get; set; }

    public string Mobile { get; set; }
}
