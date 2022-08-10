// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Identity.Tests;

[TestClass]
public class TestIdentity
{
    [TestMethod]
    public void TestIdentityClaimOptionsReturnTenantIdEqualTenantId()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentityModel(identityClaimOptions =>
        {
            identityClaimOptions.TenantId = "TenantId";
        });
        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<IdentityClaimOptions>>();
        Assert.IsTrue(optionsMonitor.CurrentValue.TenantId == "TenantId");
    }

    [TestMethod]
    public void TestIdentityModelReturnIsNotNull()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentityModel();
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IUserContext>());
        Assert.IsNotNull(serviceProvider.GetService<IMultiTenantUserContext>());
        Assert.IsNotNull(serviceProvider.GetService<IMultiEnvironmentUserContext>());
        Assert.IsNotNull(serviceProvider.GetService<IIsolatedUserContext>());
    }

    [TestMethod]
    public void TestDefaultIdentityReturnTenantIdEqualtenantid()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentityModel();
        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<IdentityClaimOptions>>();
        optionsMonitor.CurrentValue.Initialize();
        Assert.IsTrue(optionsMonitor.CurrentValue.TenantId == ClaimType.DEFAULT_TENANT_ID);
        Assert.IsTrue(optionsMonitor.CurrentValue.Environment == ClaimType.DEFAULT_ENVIRONMENT);
    }

    [TestMethod]
    public void TestAddIsolationIdentityReturnUserIdEqual1AndTenantIdEqual1()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentityModel();
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
        services.AddMasaIdentityModel();
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
                    new(ClaimType.DEFAULT_USER_ROLE, "[\"1\"]")
                })
            })
        };
        var userContext = serviceProvider.GetService<IUserContext>();
        Assert.IsNotNull(userContext);
        Assert.IsTrue(userContext.IsAuthenticated);
        Assert.IsTrue(userContext.UserId == "1");
        Assert.IsTrue(userContext.UserName == "Jim");
        Assert.IsTrue(userContext.GetUserRoles<string>().Count() == 1);

        var multiTenantUserContext = serviceProvider.GetService<IMultiTenantUserContext>();
        Assert.IsNotNull(multiTenantUserContext);

        var multiEnvironmentUserContext = serviceProvider.GetService<IMultiEnvironmentUserContext>();
        Assert.IsNotNull(multiEnvironmentUserContext);

        var isolationUserContext = serviceProvider.GetService<IIsolatedUserContext>();
        Assert.IsNotNull(isolationUserContext);
    }

    [TestMethod]
    public void TestAddMultiTenantIdentityReturnTenantIdIs1()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentityModel();
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
                    new(ClaimType.DEFAULT_USER_ROLE, "[\"1\",\"2\"]")
                })
            })
        };
        var userContext = serviceProvider.GetService<IUserContext>();
        Assert.IsNotNull(userContext);
        Assert.IsTrue(userContext.IsAuthenticated);
        Assert.IsTrue(userContext.UserId == "1");
        Assert.IsTrue(userContext.UserName == "Jim");
        Assert.IsTrue(userContext.GetUserRoles<string>().Any());

        var multiTenantUserContext = serviceProvider.GetService<IMultiTenantUserContext>();
        Assert.IsNotNull(multiTenantUserContext);
        Assert.IsTrue(multiTenantUserContext.TenantId == "1");

        var multiEnvironmentUserContext = serviceProvider.GetService<IMultiEnvironmentUserContext>();
        Assert.IsNotNull(multiEnvironmentUserContext);

        var isolationUserContext = serviceProvider.GetService<IIsolatedUserContext>();
        Assert.IsNotNull(isolationUserContext);
    }

    [TestMethod]
    public void TestTemporarilyUserReturnUserIdEqual1()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentityModel();
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

    [TestMethod]
    public void TestCustomerUserModelReturnTrueNameEqualLisi()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentityModel();
        services.Configure<IdentityClaimOptions>(option =>
        {
            option.Mapping(nameof(CustomerUser.TrueName), "realname");
        });
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
                    new("realname", "lisi")
                })
            })
        };
        var userContext = serviceProvider.GetRequiredService<IUserContext>();
        var user = userContext.GetUser<CustomerUser>();
        Assert.IsTrue(user is { TrueName: "lisi" });
    }

    [TestMethod]
    public void TestCustomerUserModel2ReturnTrueNameEqualLisi()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentityModel(option =>
        {
            option.Mapping(nameof(CustomerUser.TrueName), "realname");
        });
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
                    new("realname", "lisi")
                })
            })
        };
        var userContext = serviceProvider.GetRequiredService<IUserContext>();
        var user = userContext.GetUser<CustomerUser>();
        Assert.IsTrue(user is { TrueName: "lisi" });
    }
}
