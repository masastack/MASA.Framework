// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests;

[TestClass]
public class TestBase
{
    protected IServiceProvider ServiceProvider { get; private set; }

    protected IServiceCollection Services { get; private set; }

    public TestBase() : this(null)
    {
    }

    public TestBase(Func<IServiceCollection, IServiceCollection>? func = null) => ResetMemoryEventBus(func, false, null);

    protected void ResetMemoryEventBus(Func<IServiceCollection, IServiceCollection>? func = null, bool isAddLog = true, params Assembly[]? assemblies)
    {
        Services = new ServiceCollection();
        if (isAddLog)
        {
            Services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        }
        else
        {
            Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.Services.RemoveAll(typeof(ILogger<>));
            });
        }

        Services.AddTransient(typeof(IEventMiddleware<>), typeof(LoggingEventMiddleware<>));
        func?.Invoke(Services);

        Services.AddEventBus(assemblies?? DefaultAssemblies, ServiceLifetime.Scoped);
        ServiceProvider = Services.BuildServiceProvider();
    }

    private static Assembly[] DefaultAssemblies => new [] { typeof(TestBase).Assembly };

    protected void ResetMemoryEventBus(params Assembly[] assemblies) => ResetMemoryEventBus(null, true, assemblies);

    protected void ResetMemoryEventBus(bool isAddLog, params Assembly[] assemblies) => ResetMemoryEventBus(null, true, assemblies);
}
