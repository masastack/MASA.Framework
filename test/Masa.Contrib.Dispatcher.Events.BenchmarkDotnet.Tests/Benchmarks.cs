// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.BenchmarkDotnet.Tests;

[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 100)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class Benchmarks
{
    private RegisterUserEvent _userEvent;
    private ForgetPasswordEvent _forgetPasswordEvent;
    private IServiceProvider _serviceProvider;
    private IEventBus _eventBus;

    [GlobalSetup]
    public void GlobalSetup()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.ClearProviders());
        services.AddEventBus();
        _serviceProvider = services.BuildServiceProvider();
        _eventBus = _serviceProvider.GetRequiredService<IEventBus>();
        _userEvent = new RegisterUserEvent()
        {
            Name = "tom",
            PhoneNumber = "18888888888"
        };
        _forgetPasswordEvent = new ForgetPasswordEvent()
        {
            Name = "lisa",
            PhoneNumber = "19999999999"
        };
    }

    [Benchmark]
    public async Task Direct()
    {
        var _couponHandler = new CouponHandler(_serviceProvider);
        await _couponHandler.SendCoupon(_userEvent);
    }

    [Benchmark]
    public async Task LambdaTree()
    {
        await _eventBus.PublishAsync(_userEvent);
    }

    [Benchmark]
    public async Task SendForgetPasseordByDirect()
    {
        var emailNoticeHandler = new NoticeEmailHandler(_serviceProvider);
        var smsNoticeHandler = new NoticeSmsHandler(_serviceProvider);
        var sendCouponHandler = new SendCouponHandler(_serviceProvider);
        await emailNoticeHandler.HandleAsync(_forgetPasswordEvent);
        await smsNoticeHandler.HandleAsync(_forgetPasswordEvent);
        await sendCouponHandler.HandleAsync(_forgetPasswordEvent);
    }

    [Benchmark]
    public async Task SendForgetPasseordByInterfaces()
    {
        await _eventBus.PublishAsync(_forgetPasswordEvent);
    }
}
