using MASA.Utils.Caching.Core.Interfaces;
using MASA.Utils.Caching.Core.Models;
using MASA.Utils.Caching.DistributedMemory.Models;
using MASA.Utils.Caller.Core;
using MASA.Utils.Caller.HttpClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace MASA.Contrib.BasicAbility.Dcc.Tests;

[TestClass]
public class DccTest
{
    private string DEFAULT_CLIENT_NAME = "masa.plugins.caching.dcc";
    private Mock<IMasaConfigurationBuilder> _masaConfigurationBuilder;
    private JsonSerializerOptions _jsonSerializerOptions;
    private IServiceCollection _services;

    private Mock<IMemoryCacheClientFactory> _memoryCacheClientFactory;
    private Mock<IMemoryCache> _memoryCache;
    private Mock<IDistributedCacheClient> _distributedCacheClient;
    private const string DefaultEnvironmentName = "ASPNETCORE_ENVIRONMENT";
    private const string DEFAULT_SUBSCRIBE_KEY_PREFIX = "masa.dcc:";

    [TestInitialize]
    public void Initialize()
    {
        _masaConfigurationBuilder = new();
        _memoryCacheClientFactory = new();
        _memoryCache = new();
        _distributedCacheClient = new();
        _services = new ServiceCollection();
        _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [TestMethod]
    public void TestErrorDccSection()
    {
        _masaConfigurationBuilder.Setup(builder => builder.GetSectionRelations()).Returns(new Dictionary<string, IConfiguration>()).Verifiable();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(new ServiceCollection()));
    }

    [TestMethod]
    public void TestTryAddConfigurationAPIClient()
    {
        _memoryCacheClientFactory.Setup(factory => factory.CreateClient(DEFAULT_CLIENT_NAME)).Returns(() => null!).Verifiable();
        _services.AddSingleton(serviceProvider => _memoryCacheClientFactory.Object);
        MasaConfigurationExtensions.TryAddConfigurationAPIClient(_services, new DccSectionOptions(), new List<DccSectionOptions>(), null!);
        Assert.IsTrue(_services.Count(service => service.ServiceType == typeof(IConfigurationAPIClient) && service.Lifetime == ServiceLifetime.Singleton) == 1);
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            var clienties = _services.BuildServiceProvider().GetServices<IConfigurationAPIClient>();
        });

        _services = new ServiceCollection();
        _memoryCacheClientFactory
            .Setup(factory => factory.CreateClient(DEFAULT_CLIENT_NAME))
            .Returns(() => new MemoryCacheClient(_memoryCache.Object, _distributedCacheClient.Object, SubscribeKeyTypes.ValueTypeFullNameAndKey))
            .Verifiable();
        _services.AddSingleton(serviceProvider => _memoryCacheClientFactory.Object);
        MasaConfigurationExtensions.TryAddConfigurationAPIClient(_services, new DccSectionOptions(), new List<DccSectionOptions>(), new JsonSerializerOptions()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        var clienties = _services.BuildServiceProvider().GetServices<IConfigurationAPIClient>();
        Assert.IsTrue(clienties.Count() == 1);

        _services = new ServiceCollection();
        _memoryCacheClientFactory
            .Setup(factory => factory.CreateClient(DEFAULT_CLIENT_NAME))
                .Returns(() => new MemoryCacheClient(_memoryCache.Object, _distributedCacheClient.Object, Utils.Caching.Core.Models.SubscribeKeyTypes.ValueTypeFullNameAndKey))
                .Verifiable();
        _services.AddSingleton(serviceProvider => _memoryCacheClientFactory.Object);
        MasaConfigurationExtensions.TryAddConfigurationAPIClient(_services, new DccSectionOptions(), new List<DccSectionOptions>(), _jsonSerializerOptions);
        MasaConfigurationExtensions.TryAddConfigurationAPIClient(_services, new DccSectionOptions(), new List<DccSectionOptions>(), _jsonSerializerOptions);
        clienties = _services.BuildServiceProvider().GetServices<IConfigurationAPIClient>();
        Assert.IsTrue(clienties.Count() == 1);
    }

    [TestMethod]
    public void TestTryAddConfigurationAPIManage()
    {
        Mock<IHttpClientFactory> httpClientFactory = new();
        _services.AddSingleton(httpClientFactory.Object);
        _services.AddCaller(options => options.UseHttpClient());

        MasaConfigurationExtensions.TryAddConfigurationAPIManage(_services, new DccSectionOptions(), new List<DccSectionOptions>());
        MasaConfigurationExtensions.TryAddConfigurationAPIManage(_services, new DccSectionOptions(), new List<DccSectionOptions>());
        Assert.IsTrue(_services.Count(service => service.ServiceType == typeof(IConfigurationAPIManage) && service.Lifetime == ServiceLifetime.Singleton) == 1);
        var serviceProvider = _services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IConfigurationAPIManage>().Count() == 1);
    }

    [TestMethod]
    public void TestUseDCCAndErrorSection()
    {
        _services.AddCaller(options => options.UseHttpClient());
        _masaConfigurationBuilder.Setup(builder => builder.GetSectionRelations()).Returns(new Dictionary<string, IConfiguration>()).Verifiable();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, "", null, null), "configureOptions");
    }

    [TestMethod]
    public void TestUseDCCAndNullDccConfigurationOption()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, () => null!, option =>
        {
            option.AppId = "Test";
            option.Environment = "Test";
            option.ConfigObjects = new List<string>() { "Te" };
        }, null), "configureOptions");

        Initialize();

        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, null!, option =>
        {
            option.AppId = "Test";
            option.Environment = "Test";
            option.ConfigObjects = new List<string>() { "Te" };
        }, null), "configureOptions");
    }

    [TestMethod]
    public void TestCustomCaller()
    {
        Mock<IConfigurationAPIClient> configurationAPIClient = new();
        configurationAPIClient.Setup(client => client.GetRawAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Action<string>>()).Result).Returns(() => ("", ConfigurationTypes.Text));
        _services.AddSingleton(configurationAPIClient.Object);
        _masaConfigurationBuilder.Object.UseDcc(_services, () => new DccConfigurationOptions()
        {
            ManageServiceAddress = "https://github.com",
            RedisOptions = new Utils.Caching.Redis.Models.RedisConfigurationOptions
            {
                Servers = new List<Utils.Caching.Redis.Models.RedisServerOptions>()
                {
                    new Utils.Caching.Redis.Models.RedisServerOptions()
                    {
                        Host = "localhost",
                        Port = 6379
                    }
                }
            }
        }, option =>
        {
            option.AppId = "Test";
            option.Environment = "Test";
            option.ConfigObjects = new List<string>()
            {
                "Settings"
            };
        }, null, jsonSerializerOption =>
        {
            jsonSerializerOption.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        }, option =>
        {
            option.UseHttpClient(builder =>
            {
                builder.Name = "CustomHttpClient";
                builder.Configure = opt => opt.BaseAddress = new Uri("https://github.com");
            });
        });
        var callerProvider = _services.BuildServiceProvider().GetRequiredService<ICallerFactory>().CreateClient("CustomHttpClient");
        Assert.IsNotNull(callerProvider);
    }

    [TestMethod]
    public void TestUseDCCAndEmptyDccServiceAddress()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, () =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "",
            };
        }, null!, null), "DccServiceAddress");
    }

    [TestMethod]
    public void TestUseDCCAndErrorDccService()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, () =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new Utils.Caching.Redis.Models.RedisConfigurationOptions()
                {
                    Servers = null!
                }
            };
        }, null!, null), "Servers");

        _services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, () =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new Utils.Caching.Redis.Models.RedisConfigurationOptions
                {
                    Servers = new List<Utils.Caching.Redis.Models.RedisServerOptions>()
                }
            };
        }, null!, null), "Servers");

        _services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, () =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new Utils.Caching.Redis.Models.RedisConfigurationOptions
                {
                    Servers = new List<Utils.Caching.Redis.Models.RedisServerOptions>()
                    {
                        new Utils.Caching.Redis.Models.RedisServerOptions()
                        {
                            Host="",
                            Port=8080
                        }
                    }
                }
            };
        }, null!, null), "Servers");

        _services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, () =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new Utils.Caching.Redis.Models.RedisConfigurationOptions()
                {
                    Servers = new List<Utils.Caching.Redis.Models.RedisServerOptions>()
                    {
                        new Utils.Caching.Redis.Models.RedisServerOptions()
                        {
                            Host="localhost",
                            Port=-1
                        }
                    }
                }
            };
        }, null!, null), "Servers");
    }

    [TestMethod]
    public void TestUseDCCAndErrorDefaultSectionOption()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, () =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new Utils.Caching.Redis.Models.RedisConfigurationOptions()
                {
                    Servers = new List<Utils.Caching.Redis.Models.RedisServerOptions>()
                    {
                        new Utils.Caching.Redis.Models.RedisServerOptions()
                        {
                            Host = "localhost",
                            Port = 6379
                        }
                    }
                }
            };
        }, null!, null), "defaultSectionOptions");

        _services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, () =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new Utils.Caching.Redis.Models.RedisConfigurationOptions()
                {
                    Servers = new List<Utils.Caching.Redis.Models.RedisServerOptions>()
                    {
                        new Utils.Caching.Redis.Models.RedisServerOptions()
                        {
                            Host = "localhost",
                            Port = 6379
                        }
                    }
                }
            };
        }, option =>
        {
            option.AppId = "";
        }, null), "AppId cannot be empty");

        _services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, () =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new Utils.Caching.Redis.Models.RedisConfigurationOptions()
                {
                    Servers = new List<Utils.Caching.Redis.Models.RedisServerOptions>()
                    {
                        new Utils.Caching.Redis.Models.RedisServerOptions()
                        {
                            Host = "localhost",
                            Port = 6379
                        }
                    }
                }
            };
        }, option =>
        {
            option.AppId = "Test";
            option.ConfigObjects = null!;
        }, null), "ConfigObjects cannot be empty");

        _services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, () =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new Utils.Caching.Redis.Models.RedisConfigurationOptions()
                {
                    Servers = new List<Utils.Caching.Redis.Models.RedisServerOptions>()
                    {
                        new Utils.Caching.Redis.Models.RedisServerOptions()
                        {
                            Host = "localhost",
                            Port = 6379
                        }
                    }
                }
            };
        }, option =>
        {
            option.AppId = "Test";
            option.ConfigObjects = new List<string>();
        }, null), "ConfigObjects cannot be empty");

        _services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, () =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new Utils.Caching.Redis.Models.RedisConfigurationOptions()
                {
                    Servers = new List<Utils.Caching.Redis.Models.RedisServerOptions>()
                    {
                        new Utils.Caching.Redis.Models.RedisServerOptions()
                        {
                            Host = "localhost",
                            Port = 6379
                        }
                    }
                }
            };
        }, option =>
        {
            option.AppId = "Test";
            option.ConfigObjects = new List<string>()
            {
                "Brand"
            };
        }, null), "Error getting environment information, please make sure the value of ASPNETCORE_ENVIRONMENT has been configured");
    }

    [TestMethod]
    public void TestUseDCCAndErrorExpansionSectionOptions()
    {
        System.Environment.SetEnvironmentVariable(DefaultEnvironmentName, "Test");

        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, () =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new Utils.Caching.Redis.Models.RedisConfigurationOptions()
                {
                    Servers = new List<Utils.Caching.Redis.Models.RedisServerOptions>()
                    {
                        new Utils.Caching.Redis.Models.RedisServerOptions()
                        {
                            Host = "localhost",
                            Port = 6379
                        }
                    }
                }
            };
        }, option =>
        {
            option.AppId = "Test";
            option.ConfigObjects = new List<string>()
            {
                "Brand"
            };
        }, option =>
        {
            option.ExpandSections = new List<DccSectionOptions>()
            {
                new DccSectionOptions()
                {
                    AppId = "Test2",
                }
            };
        }), "ConfigObjects in the extension section cannot be empty");

        _services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, () =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new Utils.Caching.Redis.Models.RedisConfigurationOptions()
                {
                    Servers = new List<Utils.Caching.Redis.Models.RedisServerOptions>()
                    {
                        new Utils.Caching.Redis.Models.RedisServerOptions()
                        {
                            Host = "localhost",
                            Port = 6379
                        }
                    }
                }
            };
        }, option =>
        {
            option.AppId = "Test";
            option.ConfigObjects = new List<string>()
            {
                "Brand"
            };
        }, option =>
        {
            option.ExpandSections = new List<DccSectionOptions>()
            {
                new DccSectionOptions()
                {
                    AppId = "Test2",
                    ConfigObjects=new List<string>()
                }
            };
        }), "ConfigObjects in the extension section cannot be empty");

        _services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, () =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new Utils.Caching.Redis.Models.RedisConfigurationOptions()
                {
                    Servers = new List<Utils.Caching.Redis.Models.RedisServerOptions>()
                    {
                        new Utils.Caching.Redis.Models.RedisServerOptions()
                        {
                            Host = "localhost",
                            Port = 6379
                        }
                    }
                }
            };
        }, option =>
        {
            option.AppId = "Test";
            option.ConfigObjects = new List<string>()
            {
                "Brand"
            };
        }, option =>
        {
            option.ExpandSections = new List<DccSectionOptions>()
            {
                new DccSectionOptions()
                {
                    AppId = "Test",
                    ConfigObjects=new List<string>()
                    {
                        "Settings"
                    }
                }
            };
        }), "The current section already exists, no need to mount repeatedly");

        _services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(_services, () =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new Utils.Caching.Redis.Models.RedisConfigurationOptions()
                {
                    Servers = new List<Utils.Caching.Redis.Models.RedisServerOptions>()
                    {
                        new Utils.Caching.Redis.Models.RedisServerOptions()
                        {
                            Host = "localhost",
                            Port = 6379
                        }
                    }
                }
            };
        }, option =>
        {
            option.AppId = "Test";
            option.ConfigObjects = new List<string>()
            {
                "Brand"
            };
        }, option =>
        {
            option.ExpandSections = new List<DccSectionOptions>()
            {
                new DccSectionOptions()
                {
                    AppId = "Test2",
                    ConfigObjects=new List<string>()
                    {
                        "Settings"
                    }
                },
                new DccSectionOptions()
                {
                    AppId = "Test2",
                    ConfigObjects=new List<string>()
                    {
                        "Settings"
                    }
                }
            };
        }), "The current section already exists, no need to mount repeatedly");
    }

    [DataTestMethod]
    [DataRow("Development", "Default", "WebApplication1", "Brand")]
    public void TestUseDCCAndSuccess(string environment, string cluster, string appId, string configObject)
    {
        System.Environment.SetEnvironmentVariable(DefaultEnvironmentName, "Test");
        var brand = new Brands("Microsoft");
        Mock<IConfigurationAPIClient> configurationAPIClient = new();
        configurationAPIClient.Setup(client => client.GetRawAsync(environment, cluster, appId, configObject, It.IsAny<Action<string>>()).Result).Returns(()
            => new(brand.Serialize(_jsonSerializerOptions), ConfigurationTypes.Json)
        ).Verifiable();
        _services.AddSingleton(configurationAPIClient.Object);
        _masaConfigurationBuilder.Object.UseDcc(_services, () =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new Utils.Caching.Redis.Models.RedisConfigurationOptions()
                {
                    Servers = new List<Utils.Caching.Redis.Models.RedisServerOptions>()
                    {
                        new Utils.Caching.Redis.Models.RedisServerOptions()
                        {
                            Host = "localhost",
                            Port = 6379
                        }
                    }
                }
            };
        }, option =>
        {
            option.AppId = "Test";
            option.ConfigObjects = new List<string>()
            {
                "Brand"
            };
        }, null);
        var optionFactory = _services.BuildServiceProvider().GetRequiredService<IOptionsFactory<MasaMemoryCacheOptions>>();
        var option = optionFactory.Create(DEFAULT_CLIENT_NAME);

        Assert.IsTrue(option.SubscribeKeyType == SubscribeKeyTypes.SpecificPrefix);

        Assert.IsTrue(option.SubscribeKeyPrefix == DEFAULT_SUBSCRIBE_KEY_PREFIX);
    }

    [DataTestMethod]
    [DataRow("Development", "Default", "WebApplication1", "Brand")]
    public void TestUseDccAndSingleSection(string environment, string cluster, string appId, string configObject)
    {
        CustomTrigger trigger = new CustomTrigger(_jsonSerializerOptions);
        var brand = new Brands("Microsoft");
        var newBrand = new Brands("Masa");
        Mock<IConfigurationAPIClient> configurationAPIClient = new();
        configurationAPIClient.Setup(client => client.GetRawAsync(environment, cluster, appId, configObject, It.IsAny<Action<string>>()).Result).Returns(()
            => new(brand.Serialize(_jsonSerializerOptions), ConfigurationTypes.Json)
        ).Callback((string environment, string cluster, string appId, string configObject, Action<string> action) =>
        {
            trigger.Formats = ConfigFormats.Json;
            trigger.Content = newBrand.Serialize(_jsonSerializerOptions);
            trigger.Action = action;
        });
        _services.AddSingleton(configurationAPIClient.Object);
        var chainedConfiguration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true);

        _masaConfigurationBuilder.Setup(builder => builder.GetSectionRelations()).Returns(new Dictionary<string, IConfiguration>()
        {
            { "Appsettings",chainedConfiguration.Build() }
        }).Verifiable();

        _masaConfigurationBuilder.Object.UseDcc(_services);
        configurationAPIClient.Verify(client => client.GetRawAsync(environment, cluster, appId, configObject, It.IsAny<Action<string>>()), Times.Once);
        trigger.Execute();

        Initialize();

        configurationAPIClient = new();
        configurationAPIClient.Setup(client => client.GetRawAsync(environment, cluster, appId, configObject, It.IsAny<Action<string>>()).Result).Returns(()
            => new(new Dictionary<string, string>()
            {
                { "Id",Guid.NewGuid().ToString()},
                { "Name","Masa"}
            }.Serialize(_jsonSerializerOptions), ConfigurationTypes.Properties)
        ).Verifiable();
        _services.AddSingleton(configurationAPIClient.Object);
        chainedConfiguration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", true, true);

        _masaConfigurationBuilder.Setup(builder => builder.GetSectionRelations()).Returns(new Dictionary<string, IConfiguration>()
        {
            { "Appsettings",chainedConfiguration.Build() }
        }).Verifiable();

        _masaConfigurationBuilder.Object.UseDcc(_services);
        configurationAPIClient.Verify(client => client.GetRawAsync(environment, cluster, appId, configObject, It.IsAny<Action<string>>()), Times.Once);

        Initialize();

        configurationAPIClient = new();
        configurationAPIClient.Setup(client => client.GetRawAsync(environment, cluster, appId, configObject, It.IsAny<Action<string>>()).Result).Returns(()
            => new("Test", ConfigurationTypes.Text)
        ).Verifiable();
        _services.AddSingleton(configurationAPIClient.Object);
        chainedConfiguration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", true, true);

        _masaConfigurationBuilder.Setup(builder => builder.GetSectionRelations()).Returns(new Dictionary<string, IConfiguration>()
        {
            { "Appsettings",chainedConfiguration.Build() }
        }).Verifiable();

        _masaConfigurationBuilder.Object.UseDcc(_services);
        configurationAPIClient.Verify(client => client.GetRawAsync(environment, cluster, appId, configObject, It.IsAny<Action<string>>()), Times.Once);

        Initialize();

        configurationAPIClient = new();
        configurationAPIClient.Setup(client => client.GetRawAsync(environment, cluster, appId, configObject, It.IsAny<Action<string>>()).Result).Returns(()
            => new(null, ConfigurationTypes.Text)
        ).Verifiable();
        _services.AddSingleton(configurationAPIClient.Object);
        chainedConfiguration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", true, true);

        _masaConfigurationBuilder.Setup(builder => builder.GetSectionRelations()).Returns(new Dictionary<string, IConfiguration>()
        {
            { "Appsettings",chainedConfiguration.Build() }
        }).Verifiable();

        _masaConfigurationBuilder.Object.UseDcc(_services);
        configurationAPIClient.Verify(client => client.GetRawAsync(environment, cluster, appId, configObject, It.IsAny<Action<string>>()), Times.Once);


        Initialize();

        configurationAPIClient = new();
        configurationAPIClient.Setup(client => client.GetRawAsync(environment, cluster, appId, configObject, It.IsAny<Action<string>>()).Result).Returns(()
            => new("Test", (ConfigurationTypes)4)
        ).Verifiable();
        _services.AddSingleton(configurationAPIClient.Object);
        chainedConfiguration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", true, true);

        _masaConfigurationBuilder.Setup(builder => builder.GetSectionRelations()).Returns(new Dictionary<string, IConfiguration>()
        {
            { "Appsettings",chainedConfiguration.Build() }
        }).Verifiable();

        Assert.ThrowsException<NotSupportedException>(() => _masaConfigurationBuilder.Object.UseDcc(_services), "configurationType");
        configurationAPIClient.Verify(client => client.GetRawAsync(environment, cluster, appId, configObject, It.IsAny<Action<string>>()), Times.Once);
    }

    [TestMethod]
    public void TestUseDccAndExpandSections()
    {
        var brand = new Brands("Microsoft");
        Mock<IConfigurationAPIClient> configurationAPIClient = new();
        configurationAPIClient.Setup(client => client.GetRawAsync("Test", "Default", "DccTest", "Test1", It.IsAny<Action<string>>()).Result).Returns(()
            => new(brand.Serialize(_jsonSerializerOptions), ConfigurationTypes.Json)
        ).Verifiable();
        configurationAPIClient.Setup(client => client.GetRawAsync("Test2", "Default", "DccTest2", "Test3", It.IsAny<Action<string>>()).Result).Returns(()
           => new(brand.Serialize(_jsonSerializerOptions), ConfigurationTypes.Json)
       ).Verifiable();
        _services.AddSingleton(configurationAPIClient.Object);
        var chainedConfiguration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("expandSections.json", true, true);

        _masaConfigurationBuilder.Setup(builder => builder.GetSectionRelations()).Returns(new Dictionary<string, IConfiguration>()
        {
            { "Appsettings",chainedConfiguration.Build() }
        }).Verifiable();

        _masaConfigurationBuilder.Object.UseDcc(_services);
        configurationAPIClient.Verify(client => client.GetRawAsync("Test", "Default", "DccTest", "Test1", It.IsAny<Action<string>>()), Times.Once);
        configurationAPIClient.Verify(client => client.GetRawAsync("Test2", "Default", "DccTest2", "Test3", It.IsAny<Action<string>>()), Times.Once);
        configurationAPIClient.Verify(client => client.GetRawAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Exactly(2));
    }

    [DataTestMethod]
    [DataRow("Development", "Default", "WebApplication1", "Brand")]
    public void TestUseMultiDcc(string environment, string cluster, string appId, string configObject)
    {
        var brand = new Brands("Microsoft");
        Mock<IConfigurationAPIClient> configurationAPIClient = new();
        configurationAPIClient.Setup(client => client.GetRawAsync(environment, cluster, appId, configObject, It.IsAny<Action<string>>()).Result).Returns(()
            => new(brand.Serialize(_jsonSerializerOptions), ConfigurationTypes.Json)
        ).Verifiable();
        _services.AddSingleton(configurationAPIClient.Object);
        var chainedConfiguration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true);

        _masaConfigurationBuilder.Setup(builder => builder.GetSectionRelations()).Returns(new Dictionary<string, IConfiguration>()
        {
            { "Appsettings",chainedConfiguration.Build() }
        }).Verifiable();

        _masaConfigurationBuilder.Object.UseDcc(_services).UseDcc(_services);
        configurationAPIClient.Verify(client => client.GetRawAsync(environment, cluster, appId, configObject, It.IsAny<Action<string>>()), Times.Once);

        var httpClient = _services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient(DEFAULT_CLIENT_NAME);
        Assert.IsTrue(httpClient.BaseAddress!.ToString() == "http://localhost:6379/");
    }

}
