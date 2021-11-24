namespace MASA.Contrib.Dispatcher.Events.BenchmarkDotnet.Tests.Extensions.Middleware;

public class LoggingMiddleware<TEvent> : IMiddleware<TEvent> where TEvent : notnull, IEvent
{
    private readonly ILogger<LoggingMiddleware<TEvent>> _logger;
    public LoggingMiddleware(ILogger<LoggingMiddleware<TEvent>> logger) => _logger = logger;

    public async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        var eventType = @event.GetType();
        _logger.LogInformation("----- Handling command {CommandName} ({@Command})", eventType.FullName, @event);

        await next();
    }
}