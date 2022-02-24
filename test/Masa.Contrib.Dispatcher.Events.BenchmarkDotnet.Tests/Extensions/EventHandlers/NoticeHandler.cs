using Masa.Contrib.Dispatcher.Events.BenchmarkDotnet.Tests.Extensions.Events;

namespace Masa.Contrib.Dispatcher.Events.BenchmarkDotnet.Tests.Extensions.EventHandlers;

public class SendCouponHandler : ISagaEventHandler<ForgetPasswordEvent>
{
    private readonly ILogger<SendCouponHandler>? _logger;

    public SendCouponHandler(IServiceProvider serviceProvider) => _logger = serviceProvider.GetService<ILogger<SendCouponHandler>>();

    [EventHandler(Order = 10)]
    public Task HandleAsync(ForgetPasswordEvent @event)
    {
        _logger?.LogInformation("------Send Coupon------");
        return Task.CompletedTask;
    }

    public Task CancelAsync(ForgetPasswordEvent @event)
    {
        _logger?.LogInformation("------Cancel Coupon------");
        return Task.CompletedTask;
    }
}

public class NoticeSmsHandler : ISagaEventHandler<ForgetPasswordEvent>
{
    private readonly ILogger<NoticeSmsHandler>? _logger;

    public NoticeSmsHandler(IServiceProvider serviceProvider) => _logger = serviceProvider.GetService<ILogger<NoticeSmsHandler>>();

    [EventHandler(Order = 20)]
    public Task HandleAsync(ForgetPasswordEvent @event)
    {
        _logger?.LogInformation("------Send Sms Notice------");
        return Task.CompletedTask;
    }

    public Task CancelAsync(ForgetPasswordEvent @event)
    {
        _logger?.LogInformation("------Cancel Sms Notice------");
        return Task.CompletedTask;
    }
}

public class NoticeEmailHandler : ISagaEventHandler<ForgetPasswordEvent>
{
    private readonly ILogger<NoticeEmailHandler>? _logger;

    public NoticeEmailHandler(IServiceProvider serviceProvider) => _logger = serviceProvider.GetService<ILogger<NoticeEmailHandler>>();

    [EventHandler(Order = 30)]
    public Task HandleAsync(ForgetPasswordEvent @event)
    {
        _logger?.LogInformation("------Send Email Notice------");
        return Task.CompletedTask;
    }

    public Task CancelAsync(ForgetPasswordEvent @event)
    {
        _logger?.LogInformation("------Cancel Email Notice------");
        return Task.CompletedTask;
    }
}
