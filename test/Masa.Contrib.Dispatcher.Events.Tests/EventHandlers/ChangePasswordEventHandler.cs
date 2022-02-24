namespace Masa.Contrib.Dispatcher.Events.Tests.EventHandlers;

public class ChangePasswordEventHandler : ISagaEventHandler<ChangePasswordEvent>
{
    private readonly ILogger<ChangePasswordEventHandler>? _logger;
    public ChangePasswordEventHandler(ILogger<ChangePasswordEventHandler>? logger=null) => _logger = logger;

    [EventHandler(10, FailureLevels.ThrowAndCancel)]
    public Task HandleAsync(ChangePasswordEvent @event)
    {
        if (@event.Content.Contains("@"))
        {
            throw new ArgumentException("Invalid content parameter");
        }
        return Task.CompletedTask;
    }

    public Task CancelAsync(ChangePasswordEvent @event)
    {
        if (@event.Account.Equals("mark"))
        {
            throw new ArgumentException("System error, please try again later");
        }
        _logger?.LogInformation("cancel success");
        return Task.CompletedTask;
    }


    [EventHandler(0, FailureLevels.Ignore, IsCancel = true)]
    public Task AddCancelLogs(ChangePasswordEvent @event)
    {
        if (@event.Account.Equals("roller"))
        {
            throw new ArgumentException("System error, please try again later");
        }
        _logger?.LogInformation("cancel success");
        return Task.CompletedTask;
    }
}
