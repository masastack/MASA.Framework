// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Tests;

[TestClass]
public class UserClaimRepositoryTest
{
    [TestMethod]
    public async Task TestAddStandardUserClaimsAsync()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(provider => new OidcDbContext(provider.GetRequiredService<CustomDbContext>()));
        serviceCollection.AddScoped<IUserClaimRepository, UserClaimRepository>();
        var publisher = new Mock<IPublisher>();
        serviceCollection.TryAddSingleton(serviceProvider => publisher.Object);
        serviceCollection.AddDomainEventBus(dispatcherOptions =>
        {
            dispatcherOptions
            .UseIntegrationEventBus<IntegrationEventLogService>(options => options.UseEventLog<CustomDbContext>())
            .UseEventBus(eventBusBuilder =>
            {
            })
            .UseUoW<CustomDbContext>(dbOptions => dbOptions.UseInMemoryTestDatabase("TestAddStandardUserClaims"))
            .UseRepository<CustomDbContext>();
        });

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var userClaimRepository = serviceProvider.GetRequiredService<IUserClaimRepository>();
        await userClaimRepository.AddStandardUserClaimsAsync();

        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Set<UserClaim>().AddAsync(new("sub", "1"));
        var userClaims = await dbContext.Set<UserClaim>().ToListAsync();
        Assert.AreNotEqual(0, userClaims.Count);
    }
}
