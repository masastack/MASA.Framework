// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.Tests;

[TestClass]
public class ParserProviderTest
{
    [TestMethod]
    public async Task TestCookieParserAsync()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            Request =
            {
                Cookies = new RequestCookieCollection
                {
                    {
                        tenantKey, "1"
                    }
                }
            },
            RequestServices = serviceProvider
        };
        var provider = new CookieParserProvider();
        Assert.IsTrue(provider.Name == "Cookie");
        var handler = await provider.ResolveAsync(httpContextAccessor.HttpContext, tenantKey, tenantId =>
        {
            Assert.IsTrue(tenantId == "1");
        });
        Assert.IsTrue(handler);
    }

    [TestMethod]
    public async Task TestCookieParser2Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            Request =
            {
                Cookies = new RequestCookieCollection()
            },
            RequestServices = serviceProvider
        };
        var provider = new CookieParserProvider();
        var handler = await provider.ResolveAsync(httpContextAccessor.HttpContext, tenantKey, _ =>
        {
        });
        Assert.IsFalse(handler);
    }

    [TestMethod]
    public async Task TestCookieParser3Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var provider = new CookieParserProvider();
        var handler = await provider.ResolveAsync(services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>().HttpContext, tenantKey, _ =>
        {
        });
        Assert.IsFalse(handler);
    }

    [TestMethod]
    public async Task TestFormParserAsync()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            Request =
            {
                Form = new FormCollection(new Dictionary<string, StringValues>
                    {
                        { tenantKey, "1" }
                    }
                )
            },
            RequestServices = serviceProvider
        };
        var provider = new FormParserProvider();
        Assert.IsTrue(provider.Name == "Form");
        var handler = await provider.ResolveAsync(httpContextAccessor.HttpContext, tenantKey, tenantId =>
        {
            Assert.IsTrue(tenantId == "1");
        });
        Assert.IsTrue(handler);
    }

    [TestMethod]
    public async Task TestFormParser2Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            Request =
            {
                Form = new FormCollection(new Dictionary<string, StringValues>())
            },
            RequestServices = serviceProvider
        };
        var provider = new FormParserProvider();
        var handler = await provider.ResolveAsync(httpContextAccessor.HttpContext, tenantKey, _ =>
        {
        });
        Assert.IsFalse(handler);
    }

    [TestMethod]
    public async Task TestFormParser3Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            Request =
            {
                QueryString = QueryString.Create(tenantKey, "1")
            },
            RequestServices = serviceProvider
        };
        var provider = new FormParserProvider();
        Assert.IsTrue(provider.Name == "Form");
        var handler = await provider.ResolveAsync(httpContextAccessor.HttpContext, tenantKey, _ =>
        {
        });
        Assert.IsFalse(handler);
    }

    [TestMethod]
    public async Task TestHeaderParserAsync()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            Request =
            {
                Headers =
                {
                    { tenantKey, "1" }
                }
            },
            RequestServices = serviceProvider
        };
        var provider = new HeaderParserProvider();
        Assert.IsTrue(provider.Name == "Header");
        var handler = await provider.ResolveAsync(httpContextAccessor.HttpContext, tenantKey, tenantId =>
        {
            Assert.IsTrue(tenantId == "1");
        });
        Assert.IsTrue(handler);
    }

    [TestMethod]
    public async Task TestHeaderParser2Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            Request =
            {
                Headers = { }
            },
            RequestServices = serviceProvider
        };
        var provider = new HeaderParserProvider();
        var handler = await provider.ResolveAsync(httpContextAccessor.HttpContext, tenantKey, _ =>
        {
        });
        Assert.IsFalse(handler);
    }

    [TestMethod]
    public async Task TestHttpContextItemParserAsync()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            Items = new Dictionary<object, object?>
            {
                { tenantKey, "1" }
            },
            RequestServices = serviceProvider
        };
        var provider = new HttpContextItemParserProvider();
        Assert.IsTrue(provider.Name == "Items");
        var handler = await provider.ResolveAsync(httpContextAccessor.HttpContext, tenantKey, tenantId =>
        {
            Assert.IsTrue(tenantId == "1");
        });
        Assert.IsTrue(handler);
    }

    [TestMethod]
    public async Task TestHttpContextItemParser2Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            Items = new Dictionary<object, object?>()
        };
        var provider = new HttpContextItemParserProvider();
        var handler = await provider.ResolveAsync(httpContextAccessor.HttpContext, tenantKey, _ =>
        {
        });
        Assert.IsFalse(handler);
    }

    [TestMethod]
    public async Task TestHttpContextItemParser3Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var provider = new HttpContextItemParserProvider();
        var handler = await provider.ResolveAsync(services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>().HttpContext, tenantKey, _ =>
        {
        });
        Assert.IsFalse(handler);
    }

    [TestMethod]
    public async Task TestQueryStringParserAsync()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            Request = { QueryString = QueryString.Create(tenantKey, "1") },
            RequestServices = serviceProvider
        };
        var provider = new QueryStringParserProvider();
        Assert.IsTrue(provider.Name == "QueryString");
        var handler = await provider.ResolveAsync(httpContextAccessor.HttpContext, tenantKey, tenantId =>
        {
            Assert.IsTrue(tenantId == "1");
        });
        Assert.IsTrue(handler);
    }

    [TestMethod]
    public async Task TestQueryStringParser2Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            Request = { QueryString = new QueryString() },
            RequestServices = serviceProvider
        };
        var provider = new QueryStringParserProvider();
        var handler = await provider.ResolveAsync(httpContextAccessor.HttpContext, tenantKey, _ =>
        {
        });
        Assert.IsFalse(handler);
    }

    [TestMethod]
    public async Task TestQueryStringParser3Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var provider = new QueryStringParserProvider();
        var handler = await provider.ResolveAsync(services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>().HttpContext, tenantKey, _ =>
        {
        });
        Assert.IsFalse(handler);
    }

    [TestMethod]
    public async Task TestRouteParserAsync()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            Request =
            {
                RouteValues = new RouteValueDictionary
                {
                    { tenantKey, "1" }
                }
            },
            RequestServices = serviceProvider
        };
        var provider = new RouteParserProvider();
        Assert.IsTrue(provider.Name == "Route");
        var handler = await provider.ResolveAsync(httpContextAccessor.HttpContext, tenantKey, tenantId =>
        {
            Assert.IsTrue(tenantId == "1");
        });
        Assert.IsTrue(handler);
    }

    [TestMethod]
    public async Task TestRouteParser2Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            Request =
            {
                RouteValues = new RouteValueDictionary()
            },
            RequestServices = serviceProvider
        };
        var provider = new RouteParserProvider();
        var handler = await provider.ResolveAsync(httpContextAccessor.HttpContext, tenantKey, _ =>
        {
        });
        Assert.IsFalse(handler);
    }

    [TestMethod]
    public async Task TestRouteParser3Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var provider = new RouteParserProvider();
        var handler = await provider.ResolveAsync(services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>().HttpContext, tenantKey, _ =>
        {
        });
        Assert.IsFalse(handler);
    }

    [TestMethod]
    public async Task TestEnvironmentVariablesParserAsync()
    {
        var services = new ServiceCollection();
        string environmentKey = "env";
        System.Environment.SetEnvironmentVariable(environmentKey, "dev");
        var environmentVariablesParserProvider = new EnvironmentVariablesParserProvider();
        var handler = await environmentVariablesParserProvider.ResolveAsync(null, environmentKey, environment =>
        {
            Assert.IsTrue(environment == "dev");
        });
        Assert.IsTrue(environmentVariablesParserProvider.Name == "EnvironmentVariables");
        Assert.IsTrue(handler);
    }
}
