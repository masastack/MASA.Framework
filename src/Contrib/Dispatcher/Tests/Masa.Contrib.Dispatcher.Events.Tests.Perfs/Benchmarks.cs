// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Perfs;

[MarkdownExporter, AsciiDocExporter, HtmlExporter]
[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 100)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class Benchmarks
{
    private RegisterUserEvent _userEvent;
    private AddShoppingCartEvent _shoppingCartEvent;
    private IServiceProvider _serviceProvider;
    private IEventBus _eventBus;
    private IMediator _mediator;

    [GlobalSetup]
    public void GlobalSetup()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.ClearProviders());
        services.AddMediatR(typeof(Benchmarks));
        services.AddEventBus();
        _serviceProvider = services.BuildServiceProvider();
        _eventBus = _serviceProvider.GetRequiredService<IEventBus>();
        _mediator = _serviceProvider.GetRequiredService<IMediator>();
        _userEvent = new RegisterUserEvent()
        {
            Name = "tom",
            PhoneNumber = "18888888888"
        };
        _shoppingCartEvent = new AddShoppingCartEvent()
        {
            Count = 1,
            GoodsId = "Microsoft"
        };
    }

    [Benchmark(Baseline = true)]
    public async Task SendCouponByDirect()
    {
        var _couponHandler = new CouponHandler(_serviceProvider);
        await _couponHandler.SendCoupon(_userEvent);
        await _couponHandler.SendNotice(_userEvent);
    }

    [Benchmark]
    public async Task SendCouponByEventBus()
    {
        await _eventBus.PublishAsync(_userEvent);
    }

    [Benchmark]
    public async Task AddShoppingCartByEventBusAsync()
    {
        await _eventBus.PublishAsync(_shoppingCartEvent);
    }

    [Benchmark]
    public async Task AddShoppingCartByMediatRAsync()
    {
        await _mediator.Send(_shoppingCartEvent);
    }
}
