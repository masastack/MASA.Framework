namespace MASA.Contrib.Dispatcher.InMemory.Strategies;

public interface IExecutionStrategy
{
    Task ExecuteAsync<TEvent>(StrategyOptions strategyOptions, TEvent @event, Func<TEvent, Task> func, Func<TEvent, Exception, FailureLevels, Task> cancel)
        where TEvent : IEvent;
}