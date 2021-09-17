namespace MASA.Contrib.Dispatcher.InMemory.Tests.EventHandlers;

public class TransferEventHandler : ISagaEventHandler<TransferEvent>
{
    private readonly List<string> _blackAccount = new List<string>() { "roller", "thomas" };

    private readonly ILogger<TransferEventHandler> _logger;

    public TransferEventHandler(ILogger<TransferEventHandler> logger) => _logger = logger;

    [EventHandler(EnableRetry = true, RetryTimes = 3)]
    public Task HandleAsync(TransferEvent @event)
    {
        if (_blackAccount.Contains(@event.Account))
        {
            throw new NotSupportedException("System error, please try again later");
        }
        _logger.LogInformation("deduct account balance {event}", @event.ToString());
        return Task.CompletedTask;
    }

    [EventHandler(EnableRetry = true, RetryTimes = 3)]
    public Task CancelAsync(TransferEvent @event)
    {
        if (@event.Price > 1000000)
        {
            throw new NotSupportedException("Large transfer returns are not supported.");
        }
        else
        {
            return Task.CompletedTask;
        }
    }
}

public class ReceiveTransferHandler
{
    private readonly List<string> _blackAccount = new List<string>() { "clark", "evan" };

    private readonly ILogger<ReceiveTransferHandler> _logger;

    public ReceiveTransferHandler(ILogger<ReceiveTransferHandler> logger) => _logger = logger;

    [EventHandler(EnableRetry = true, RetryTimes = 3, FailureLevels = FailureLevels.ThrowAndCancel)]
    public Task HandleAsync(TransferEvent @event)
    {
        if (_blackAccount.Contains(@event.OptAccount))
        {
            throw new NotSupportedException("System error, please try again later");
        }
        _logger.LogInformation("add opt account balance");
        return Task.CompletedTask;
    }
}