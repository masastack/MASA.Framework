// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Framework.IntegrationTests.EventBus;

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
                .UseEventLog<CustomDbContext>()
                .UseEventBus(eventBusBuilder => eventBusBuilder.UseMiddleware(typeof(RecordMiddleware<>)).UseMiddleware(typeof(ValidatorMiddleware<>)))
                .UseUoW<CustomDbContext>(optionBuilder =>
                {
                    optionBuilder.UseTestSqlite($"data source=disabled-soft-delete-db-{Guid.NewGuid()}").UseFilter();
                })
                .UseRepository<CustomDbContext>();
        });
        var dbContext = ServiceProvider.GetRequiredService<CustomDbContext>();
        dbContext.Database.EnsureCreated();
    }
}
