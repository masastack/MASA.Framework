// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests.Internal;

public class CustomizeProcessor : ProcessorBase
{
    internal static int Times = 0;

    public override int Delay => 2;

    public CustomizeProcessor(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Times++;
        return Task.CompletedTask;
    }
}
