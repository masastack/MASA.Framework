// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Benchmarks.Extensions.EventHandlers;

public class CouponHandler
{
    private readonly ILogger<CouponHandler>? _logger;

    public CouponHandler(IServiceProvider serviceProvider) => _logger = serviceProvider.GetService<ILogger<CouponHandler>>();

    [EventHandler(Order = 10, FailureLevels = FailureLevels.ThrowAndCancel)]
    public Task SendCoupon(RegisterUserEvent @event)
    {
        _logger?.LogInformation("------Send Coupon------");

        var num = new Random().Next(1, 3);
        if (num % 2 == 0)
        {
            //throw new Exception("Failed to send coupons");
        }

        return Task.CompletedTask;
    }

    [EventHandler(Order = 20, FailureLevels = FailureLevels.Ignore)]
    public Task SendNotice(RegisterUserEvent @event)
    {
        _logger?.LogInformation("------Send Coupon------");

        var num = new Random().Next(1, 3);
        if (num % 2 == 0)
        {
            //throw new Exception("Failed to send coupons");
        }

        return Task.CompletedTask;
    }

    [EventHandler(Order = 10, RetryTimes = 5, IsCancel = true)]
    public Task CancelSendCoupon(RegisterUserEvent @event)
    {
        _logger?.LogInformation("------Cancel Send Coupon------");

        var num = new Random().Next(1, 3);
        if (num % 2 == 0)
        {
            //throw new Exception("Failed to cancel send coupons");
        }

        return Task.CompletedTask;
    }
}
