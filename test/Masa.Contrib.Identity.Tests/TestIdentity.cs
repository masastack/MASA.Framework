// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Identity.Tests;

[TestClass]
public class TestIdentity
{
    [TestMethod]
    public void TestIdentityClaimOptionsReturnTenantIdEqualTenantId()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentity(identityClaimOptions =>
        {
            identityClaimOptions.TenantId = "TenantId";
        });
        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<IdentityClaimOptions>>();
        Assert.IsTrue(optionsMonitor.CurrentValue.TenantId == "TenantId");
        Assert.IsTrue(optionsMonitor.CurrentValue.Environment == "environment");
    }

    [TestMethod]
    public void TestDefaultIdentityReturnTenantIdEqualtenantid()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentity();
        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<IdentityClaimOptions>>();
        Assert.IsTrue(optionsMonitor.CurrentValue.TenantId == "tenantid");
        Assert.IsTrue(optionsMonitor.CurrentValue.Environment == "environment");
    }

    [TestMethod]
    public void TestReturnUserIdEqual1()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentity();
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            User = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new(new List<Claim>()
                {
                    new(ClaimTypes.NameIdentifier, "1"),
                    new(ClaimTypes.Name, "Jim"),
                    new("tenantid", "1")
                })
            })
        };
        var userContext = serviceProvider.GetRequiredService<IUserContext>();
        Assert.IsTrue(userContext.IsAuthenticated);
        Assert.IsTrue(userContext.UserId == "1");
        Assert.IsTrue(userContext.UserName == "Jim");
        Assert.IsTrue(userContext.TenantId == "1");
        Assert.IsNull(userContext.Environment);
    }

    [TestMethod]
    public void TestTemporarilyUserReturnUserIdEqual1()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentity();
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            User = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new(new List<Claim>()
                {
                    new(ClaimTypes.NameIdentifier, "1"),
                    new(ClaimTypes.Name, "Jim"),
                    new("tenantid", "1")
                })
            })
        };
        var userContext = serviceProvider.GetRequiredService<IUserContext>();
        var userSetter = serviceProvider.GetRequiredService<IUserSetter>();

        using (userSetter.Change(new IdentityUser()
               {
                   Id = "2",
                   UserName = "Tom",
                   Environment = "Production",
                   TenantId = "2"
               }))
        {
            Assert.IsTrue(userContext.IsAuthenticated);
            Assert.IsTrue(userContext.UserId == "2");
            Assert.IsTrue(userContext.UserName == "Tom");
            Assert.IsTrue(userContext.TenantId == "2");
            Assert.IsTrue(userContext.Environment == "Production");
        }

        Assert.IsTrue(userContext.IsAuthenticated);
        Assert.IsTrue(userContext.UserId == "1");
        Assert.IsTrue(userContext.UserName == "Jim");
        Assert.IsTrue(userContext.TenantId == "1");
        Assert.IsNull(userContext.Environment);
    }
}
