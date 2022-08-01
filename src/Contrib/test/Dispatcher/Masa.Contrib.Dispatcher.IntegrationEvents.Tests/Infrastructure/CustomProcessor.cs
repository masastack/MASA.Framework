// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests.Infrastructure;

public class CustomProcessor : ProcessorBase
{
    public static int Times = 0;

    public override int Delay => 2;

    public CustomProcessor(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Times++;
        return Task.CompletedTask;
    }
}
