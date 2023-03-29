// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Tests;

[TestClass]
public class OidcDbContextOptionsTest
{
    [TestMethod]
    public async Task TestSeedStandardResources1Async()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(dbContext => { dbContext.UseInMemoryDatabase(Guid.NewGuid().ToString()); });

        Mock<IUserClaimRepository> userClaimRepository = new();
        userClaimRepository.Setup(u => u.AddStandardUserClaimsAsync()).Verifiable();

        Mock<IIdentityResourceRepository> identityResourceRepository = new();
        identityResourceRepository.Setup(u => u.AddStandardIdentityResourcesAsync()).Verifiable();
        services.AddScoped(serviceProvider => new OidcDbContext(serviceProvider.GetRequiredService<CustomDbContext>()));

        services.AddScoped(_ => userClaimRepository.Object);
        services.AddScoped(_ => identityResourceRepository.Object);
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();

        var oidcDbContextOptions = new OidcDbContextOptions(serviceProvider);
        await oidcDbContextOptions.SeedStandardResourcesAsync();

        userClaimRepository.Verify(r => r.AddStandardUserClaimsAsync(), Times.Once);
        identityResourceRepository.Verify(r => r.AddStandardIdentityResourcesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task TestSeedStandardResources2Async()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(dbContext => { dbContext.UseInMemoryDatabase(Guid.NewGuid().ToString()); });

        Mock<IUserClaimRepository> userClaimRepository = new();
        userClaimRepository.Setup(u => u.AddStandardUserClaimsAsync()).Verifiable();

        Mock<IIdentityResourceRepository> identityResourceRepository = new();
        identityResourceRepository.Setup(u => u.AddStandardIdentityResourcesAsync()).Verifiable();
        services.AddScoped(serviceProvider => new OidcDbContext(serviceProvider.GetRequiredService<CustomDbContext>()));

        services.AddScoped(_ => userClaimRepository.Object);
        services.AddScoped(_ => identityResourceRepository.Object);
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Set<UserClaim>().AddAsync(new("sub", "1"));
        await dbContext.Set<IdentityResource>().AddAsync(new("email", "email", "email", true, true, default, true, true));
        await dbContext.SaveChangesAsync();

        var oidcDbContextOptions = new OidcDbContextOptions(serviceProvider);
        await oidcDbContextOptions.SeedStandardResourcesAsync();

        userClaimRepository.Verify(r => r.AddStandardUserClaimsAsync(), Times.Never);
        identityResourceRepository.Verify(r => r.AddStandardIdentityResourcesAsync(), Times.Never);
    }
}
