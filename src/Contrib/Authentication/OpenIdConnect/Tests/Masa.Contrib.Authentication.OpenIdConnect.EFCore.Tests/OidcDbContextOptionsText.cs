// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Tests;

[TestClass]
public class ThirdPartyIdpServiceTest
{
    [TestMethod]
    public async Task TestSeedStandardResources1Async()
    {
        var serviceCollection = new ServiceCollection();
        var publisher = new Mock<IPublisher>();
        serviceCollection.TryAddSingleton(serviceProvider => publisher.Object);
        serviceCollection.AddDomainEventBus(dispatcherOptions =>
        {
            dispatcherOptions
            .UseIntegrationEventBus<IntegrationEventLogService>(options => options.UseEventLog<TestDbContext>())
            .UseEventBus(eventBusBuilder =>
            {
            })
            .UseUoW<TestDbContext>(
                dbOptions => dbOptions.UseInMemoryTestDatabase("TestSeedStandardResources1"), false, false
            )
            .UseRepository<TestDbContext>();
        });
        serviceCollection.AddScoped(provider => new OidcDbContext(provider.GetRequiredService<TestDbContext>()));

        var userClaimRepository = new Mock<IUserClaimRepository>();
        userClaimRepository.Setup(provider => provider.AddStandardUserClaimsAsync()).Verifiable();
        serviceCollection.AddScoped(provider => userClaimRepository.Object);

        var identityResourcerepository = new Mock<IIdentityResourceRepository>();
        identityResourcerepository.Setup(provider => provider.AddStandardIdentityResourcesAsync()).Verifiable();
        serviceCollection.AddScoped(provider => identityResourcerepository.Object);

        serviceCollection.AddScoped<OidcDbContextOptions>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<OidcDbContextOptions>();
        await options.SeedStandardResourcesAsync();
        userClaimRepository.Verify(provider => provider.AddStandardUserClaimsAsync(), Times.Once);
        identityResourcerepository.Verify(provider => provider.AddStandardIdentityResourcesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task TestSeedStandardResources2Async()
    {
        var serviceCollection = new ServiceCollection();
        var publisher = new Mock<IPublisher>();
        serviceCollection.TryAddSingleton(serviceProvider => publisher.Object);
        serviceCollection.AddDomainEventBus(dispatcherOptions =>
        {
            dispatcherOptions
            .UseIntegrationEventBus<IntegrationEventLogService>(options => options.UseEventLog<TestDbContext>())
            .UseEventBus(eventBusBuilder =>
            {
            })
            .UseUoW<TestDbContext>(
                dbOptions => dbOptions.UseInMemoryTestDatabase("TestSeedStandardResources2"), false, false
            )
            .UseRepository<TestDbContext>();
        });
        serviceCollection.AddScoped(provider => new OidcDbContext(provider.GetRequiredService<TestDbContext>()));

        var userClaimRepository = new Mock<IUserClaimRepository>();
        userClaimRepository.Setup(provider => provider.AddStandardUserClaimsAsync()).Verifiable();
        serviceCollection.AddScoped(provider => userClaimRepository.Object);

        var identityResourcerepository = new Mock<IIdentityResourceRepository>();
        identityResourcerepository.Setup(provider => provider.AddStandardIdentityResourcesAsync()).Verifiable();
        serviceCollection.AddScoped(provider => identityResourcerepository.Object);

        serviceCollection.AddScoped<OidcDbContextOptions>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<TestDbContext>();
        await dbContext.Set<UserClaim>().AddAsync(new("sub","1"));
        await dbContext.Set<IdentityResource>().AddAsync(new("email","email","email",true,true,default,true,true));
        await dbContext.SaveChangesAsync();
        var options = serviceProvider.GetRequiredService<OidcDbContextOptions>();
        await options.SeedStandardResourcesAsync();
        userClaimRepository.Verify(provider => provider.AddStandardUserClaimsAsync(), Times.Never);
        identityResourcerepository.Verify(provider => provider.AddStandardIdentityResourcesAsync(), Times.Never);
    }
}
