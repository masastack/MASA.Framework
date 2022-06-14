// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Identity.IdentityModel.Tests;

[TestClass]
public class TestIdentity
{
    [TestMethod]
    public void TestIdentityClaimOptionsReturnTenantIdEqualTenantId()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentity(IdentityType.Basic | IdentityType.MultiTenant | IdentityType.MultiEnvironment, identityClaimOptions =>
        {
            identityClaimOptions.TenantId = "TenantId";
        });
        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<IdentityClaimOptions>>();
        Assert.IsTrue(optionsMonitor.CurrentValue.TenantId == "TenantId");
        Assert.IsTrue(optionsMonitor.CurrentValue.Environment == ClaimType.DEFAULT_ENVIRONMENT);
    }

    [TestMethod]
    public void TestIdentityType()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentity(IdentityType.Basic);
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNull(serviceProvider.GetService<IMultiTenantUserContext>());
        Assert.IsNull(serviceProvider.GetService<IMultiEnvironmentIdentityUser>());

        services = new ServiceCollection();
        services.AddMasaIdentity(IdentityType.MultiTenant);
        serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IMultiTenantUserContext>());
        Assert.IsNull(serviceProvider.GetService<IMultiEnvironmentIdentityUser>());

        services = new ServiceCollection();
        services.AddMasaIdentity(IdentityType.Basic | IdentityType.MultiTenant);
        serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IMultiTenantUserContext>());
        Assert.IsNull(serviceProvider.GetService<IMultiEnvironmentIdentityUser>());

        services = new ServiceCollection();
        services.AddMasaIdentity(IdentityType.MultiEnvironment);
        serviceProvider = services.BuildServiceProvider();
        Assert.IsNull(serviceProvider.GetService<IMultiTenantUserContext>());
        Assert.IsNotNull(serviceProvider.GetService<IMultiEnvironmentUserContext>());

        services = new ServiceCollection();
        services.AddMasaIdentity(IdentityType.Basic | IdentityType.MultiEnvironment);
        serviceProvider = services.BuildServiceProvider();
        Assert.IsNull(serviceProvider.GetService<IMultiTenantUserContext>());
        Assert.IsNotNull(serviceProvider.GetService<IMultiEnvironmentUserContext>());

        services = new ServiceCollection();
        services.AddMasaIdentity(IdentityType.MultiTenant | IdentityType.MultiEnvironment);
        serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IMultiTenantUserContext>());
        Assert.IsNotNull(serviceProvider.GetService<IMultiEnvironmentUserContext>());

        services = new ServiceCollection();
        services.AddMasaIdentity(IdentityType.Basic | IdentityType.MultiTenant | IdentityType.MultiEnvironment);
        serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IMultiTenantUserContext>());
        Assert.IsNotNull(serviceProvider.GetService<IMultiEnvironmentUserContext>());
        Assert.IsNotNull(serviceProvider.GetService<IIsolatedUserContext>());
    }

    [TestMethod]
    public void TestDefaultIdentityReturnTenantIdEqualtenantid()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentity();
        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<IdentityClaimOptions>>();
        Assert.IsTrue(optionsMonitor.CurrentValue.TenantId == ClaimType.DEFAULT_TENANT_ID);
        Assert.IsTrue(optionsMonitor.CurrentValue.Environment == ClaimType.DEFAULT_ENVIRONMENT);
    }

    [TestMethod]
    public void TestAddIsolationIdentityReturnUserIdEqual1AndTenantIdEqual1()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentity(IdentityType.Basic | IdentityType.MultiTenant | IdentityType.MultiEnvironment);
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            User = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new(new List<Claim>()
                {
                    new(ClaimType.DEFAULT_USER_ID, "1"),
                    new(ClaimType.DEFAULT_USER_NAME, "Jim"),
                    new(ClaimType.DEFAULT_TENANT_ID, "1"),
                    new(ClaimType.DEFAULT_ENVIRONMENT, "dev")
                })
            })
        };
        var userContext = serviceProvider.GetService<IUserContext>();
        Assert.IsNotNull(userContext);

        Assert.IsTrue(userContext.UserId == "1");

        var multiTenantUserContext = serviceProvider.GetService<IMultiTenantUserContext>();
        Assert.IsNotNull(multiTenantUserContext);

        Assert.IsTrue(multiTenantUserContext.TenantId == "1");

        var multiEnvironmentUserContext = serviceProvider.GetService<IMultiEnvironmentUserContext>();
        Assert.IsNotNull(multiEnvironmentUserContext);

        Assert.IsTrue(multiEnvironmentUserContext.Environment == "dev");

        var isolationUserContext = serviceProvider.GetService<IIsolatedUserContext>();
        Assert.IsNotNull(isolationUserContext);

        Assert.IsTrue(isolationUserContext.IsAuthenticated);
        Assert.IsTrue(isolationUserContext.UserId == "1");
        Assert.IsTrue(isolationUserContext.UserName == "Jim");
        Assert.IsTrue(isolationUserContext.TenantId == "1");
        Assert.IsTrue(isolationUserContext.Environment == "dev");
    }

    [TestMethod]
    public void TestAddSimpleIdentityReturnUserIdEqual1()
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
                    new(ClaimType.DEFAULT_USER_ID, "1"),
                    new(ClaimType.DEFAULT_USER_NAME, "Jim"),
                    new(ClaimType.DEFAULT_TENANT_ID, "1")
                })
            })
        };
        var userContext = serviceProvider.GetService<IUserContext>();
        Assert.IsNotNull(userContext);
        Assert.IsTrue(userContext.IsAuthenticated);
        Assert.IsTrue(userContext.UserId == "1");
        Assert.IsTrue(userContext.UserName == "Jim");

        var multiTenantUserContext = serviceProvider.GetService<IMultiTenantUserContext>();
        Assert.IsNull(multiTenantUserContext);

        var multiEnvironmentUserContext = serviceProvider.GetService<IMultiEnvironmentUserContext>();
        Assert.IsNull(multiEnvironmentUserContext);

        var isolationUserContext = serviceProvider.GetService<IIsolatedUserContext>();
        Assert.IsNull(isolationUserContext);
    }

    [TestMethod]
    public void TestAddMultiTenantIdentityReturnTenantIdIs1()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentity(IdentityType.MultiTenant);
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            User = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new(new List<Claim>()
                {
                    new(ClaimType.DEFAULT_USER_ID, "1"),
                    new(ClaimType.DEFAULT_USER_NAME, "Jim"),
                    new(ClaimType.DEFAULT_TENANT_ID, "1")
                })
            })
        };
        var userContext = serviceProvider.GetService<IUserContext>();
        Assert.IsNotNull(userContext);
        Assert.IsTrue(userContext.IsAuthenticated);
        Assert.IsTrue(userContext.UserId == "1");
        Assert.IsTrue(userContext.UserName == "Jim");

        var multiTenantUserContext = serviceProvider.GetService<IMultiTenantUserContext>();
        Assert.IsNotNull(multiTenantUserContext);
        Assert.IsTrue(multiTenantUserContext.TenantId == "1");

        var multiEnvironmentUserContext = serviceProvider.GetService<IMultiEnvironmentUserContext>();
        Assert.IsNull(multiEnvironmentUserContext);

        var isolationUserContext = serviceProvider.GetService<IIsolatedUserContext>();
        Assert.IsNull(isolationUserContext);
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
                    new(ClaimType.DEFAULT_USER_ID, "1"),
                    new(ClaimType.DEFAULT_USER_NAME, "Jim")
                })
            })
        };
        var userContext = serviceProvider.GetRequiredService<IUserContext>();
        var userSetter = serviceProvider.GetRequiredService<IUserSetter>();

        var user = new IdentityUser()
        {
            Id = "2",
            UserName = "Tom"
        };
        using (userSetter.Change(user))
        {
            Assert.IsTrue(userContext.IsAuthenticated);
            Assert.IsTrue(userContext.UserId == "2");
            Assert.IsTrue(userContext.UserName == "Tom");
        }

        Assert.IsTrue(userContext.IsAuthenticated);
        Assert.IsTrue(userContext.UserId == "1");
        Assert.IsTrue(userContext.UserName == "Jim");
    }
}
