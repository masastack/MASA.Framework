// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Tests;

[TestClass]
public class TestBase
{
    protected IServiceCollection Services { get; private set; }

    protected IServiceProvider ServiceProvider => Services.BuildServiceProvider();

    [TestInitialize]
    public void Initialize()
    {
        Services = new ServiceCollection();
        Services.AddFluentValidation(options =>
        {
            options.RegisterValidatorsFromAssemblyContaining<User>();
        });
        Services.AddDomainEventBus(dispatcherOptions =>
        {
            dispatcherOptions
                .UseIntegrationEventBus<IntegrationEventLogService>(option => option.UseTestPub())
                .UseEventLog<CustomizeDbContext>()
                .UseEventBus(eventBusBuilder => eventBusBuilder.UseMiddleware(typeof(RecordMiddleware<>)).UseMiddleware(typeof(ValidatorMiddleware<>)))
                .UseUoW<CustomizeDbContext>(optionBuilder =>
                {
                    optionBuilder.UseTestSqlite($"data source=disabled-soft-delete-db-{Guid.NewGuid()}").UseTestFilter();
                })
                .UseRepository<CustomizeDbContext>();
        });
        var dbContext = ServiceProvider.GetRequiredService<CustomizeDbContext>();
        dbContext.Database.EnsureCreated();
    }
}
