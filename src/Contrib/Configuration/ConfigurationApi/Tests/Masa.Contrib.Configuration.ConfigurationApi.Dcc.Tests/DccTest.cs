// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests;

[TestClass]
public class DccTest
{
    private string DEFAULT_CLIENT_NAME = "masa.contrib.configuration.configurationapi.dcc";
    private Mock<IMasaConfigurationBuilder> _masaConfigurationBuilder;
    private JsonSerializerOptions _jsonSerializerOptions;
    private IServiceCollection _services;

    private Mock<IMemoryCacheClientFactory> _memoryCacheClientFactory;
    private Mock<IMemoryCache> _memoryCache;
    private Mock<IDistributedCacheClient> _distributedCacheClient;
    private const string DEFAULT_ENVIRONMENT_NAME = "ASPNETCORE_ENVIRONMENT";
    private const string DEFAULT_SUBSCRIBE_KEY_PREFIX = "masa.dcc:";

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
        _masaConfigurationBuilder = new Mock<IMasaConfigurationBuilder>();
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build;
        _masaConfigurationBuilder.Setup(builder => builder.Configuration).Returns(configuration).Verifiable();
        _masaConfigurationBuilder.Setup(builder => builder.Services).Returns(_services).Verifiable();
        _memoryCacheClientFactory = new Mock<IMemoryCacheClientFactory>();
        _memoryCache = new Mock<IMemoryCache>();
        _distributedCacheClient = new Mock<IDistributedCacheClient>();
        _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [TestMethod]
    public void TestTryAddConfigurationApiClient()
    {
        _memoryCacheClientFactory.Setup(factory => factory.CreateClient(DEFAULT_CLIENT_NAME)).Returns(() => null!).Verifiable();
        _services.AddSingleton(_ => _memoryCacheClientFactory.Object);
        MasaConfigurationExtensions.TryAddConfigurationApiClient(_services, new DccSectionOptions(), new List<DccSectionOptions>(), null!);
        Assert.IsTrue(_services.Count(service
            => service.ServiceType == typeof(IConfigurationApiClient) && service.Lifetime == ServiceLifetime.Singleton) == 1);
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            var _ = _services.BuildServiceProvider().GetServices<IConfigurationApiClient>();
        });

        _services = new ServiceCollection();
        _memoryCacheClientFactory
            .Setup(factory => factory.CreateClient(DEFAULT_CLIENT_NAME))
            .Returns(() => new MemoryCacheClient(_memoryCache.Object, _distributedCacheClient.Object,
                SubscribeKeyTypes.ValueTypeFullNameAndKey))
            .Verifiable();
        _services.AddSingleton(_ => _memoryCacheClientFactory.Object);
        MasaConfigurationExtensions.TryAddConfigurationApiClient(_services, new DccSectionOptions(), new List<DccSectionOptions>(),
            new JsonSerializerOptions()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

        var clienties = _services.BuildServiceProvider().GetServices<IConfigurationApiClient>();
        Assert.IsTrue(clienties.Count() == 1);

        _services = new ServiceCollection();
        _memoryCacheClientFactory
            .Setup(factory => factory.CreateClient(DEFAULT_CLIENT_NAME))
            .Returns(() => new MemoryCacheClient(_memoryCache.Object, _distributedCacheClient.Object,
                SubscribeKeyTypes.ValueTypeFullNameAndKey))
            .Verifiable();
        _services.AddSingleton(_ => _memoryCacheClientFactory.Object);
        MasaConfigurationExtensions.TryAddConfigurationApiClient(_services, new DccSectionOptions(), new List<DccSectionOptions>(),
            _jsonSerializerOptions);
        MasaConfigurationExtensions.TryAddConfigurationApiClient(_services, new DccSectionOptions(), new List<DccSectionOptions>(),
            _jsonSerializerOptions);
        clienties = _services.BuildServiceProvider().GetServices<IConfigurationApiClient>();
        Assert.IsTrue(clienties.Count() == 1);
    }

    [TestMethod]
    public void TestTryAddConfigurationApiManage()
    {
        Mock<IHttpClientFactory> httpClientFactory = new();
        _services.AddSingleton(httpClientFactory.Object);
        _services.AddCaller(options => options.UseHttpClient());

        MasaConfigurationExtensions.TryAddConfigurationApiManage(
            _services,
            Microsoft.Extensions.Options.Options.DefaultName,
            new DccSectionOptions(),
            new List<DccSectionOptions>());

        MasaConfigurationExtensions.TryAddConfigurationApiManage(
            _services,
            Microsoft.Extensions.Options.Options.DefaultName,
            new DccSectionOptions(),
            new List<DccSectionOptions>());
        Assert.IsTrue(_services.Count(service
            => service.ServiceType == typeof(IConfigurationApiManage) && service.Lifetime == ServiceLifetime.Singleton) == 1);
        var serviceProvider = _services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IConfigurationApiManage>().Count() == 1);
    }

    [TestMethod]
    public void TestUseDccAndNullDccConfigurationOption()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(() => null!, option =>
        {
            option.AppId = "Test";
            option.Environment = "Test";
            option.ConfigObjects = new List<string>() { "Te" };
        }, null));
    }

    [TestMethod]
    public void TestCustomCaller()
    {
        var response = JsonSerializer.Serialize(new PublishRelease()
        {
            Content = string.Empty,
            ConfigFormat = ConfigFormats.Text
        });
        Mock<IMemoryCacheClient> memoryCacheClient = new();
        memoryCacheClient.Setup(client => client.GetAsync(It.IsAny<string>(), It.IsAny<Action<string?>>()).Result)
            .Returns(() => response);

        var configurationApiClient = new ConfigurationApiClient(_services.BuildServiceProvider(),
            memoryCacheClient.Object, _jsonSerializerOptions, new Mock<DccSectionOptions>().Object, new List<DccSectionOptions>());
        _services.AddSingleton<IConfigurationApiClient>(configurationApiClient);
        _masaConfigurationBuilder.Object.UseDcc(() => new DccConfigurationOptions()
        {
            ManageServiceAddress = "https://github.com",
            RedisOptions = new RedisConfigurationOptions
            {
                Servers = new List<RedisServerOptions>()
                {
                    new()
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
            option.UseHttpClient("CustomHttpClient", builder =>
            {
                builder.Configure = opt => opt.BaseAddress = new Uri("https://github.com");
            });
        });
        var caller = _services.BuildServiceProvider().GetRequiredService<ICallerFactory>().Create("CustomHttpClient");
        Assert.IsNotNull(caller);
    }

    [TestMethod]
    public void TestUseDccAndEmptyDccServiceAddress()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(() =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "",
            };
        }, null!, null), "DccServiceAddress");
    }

    [TestMethod]
    public void TestUseDccAndErrorDccService()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(() =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new RedisConfigurationOptions()
                {
                    Servers = null!
                }
            };
        }, null!, null), "Servers");

        _services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(() =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new RedisConfigurationOptions
                {
                    Servers = new List<RedisServerOptions>()
                }
            };
        }, null!, null), "Servers");

        _services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(() =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new RedisConfigurationOptions
                {
                    Servers = new List<RedisServerOptions>()
                    {
                        new()
                        {
                            Host = "",
                            Port = 8080
                        }
                    }
                }
            };
        }, null!, null), "Servers");

        _services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(() =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new RedisConfigurationOptions()
                {
                    Servers = new List<RedisServerOptions>()
                    {
                        new()
                        {
                            Host = "localhost",
                            Port = -1
                        }
                    }
                }
            };
        }, null!, null), "Servers");
    }

    [TestMethod]
    public void TestUseDccAndErrorDefaultSectionOption()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(() =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new RedisConfigurationOptions()
                {
                    Servers = new List<RedisServerOptions>()
                    {
                        new()
                        {
                            Host = "localhost",
                            Port = 6379
                        }
                    }
                }
            };
        }, null!, null), "defaultSectionOptions");

        _services = new ServiceCollection();
        _masaConfigurationBuilder.Setup(builder => builder.Services).Returns(_services).Verifiable();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(() =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new RedisConfigurationOptions()
                {
                    Servers = new List<RedisServerOptions>()
                    {
                        new()
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
        _masaConfigurationBuilder.Setup(builder => builder.Services).Returns(_services).Verifiable();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(() =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new RedisConfigurationOptions()
                {
                    Servers = new List<RedisServerOptions>()
                    {
                        new()
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
        _masaConfigurationBuilder.Setup(builder => builder.Services).Returns(_services).Verifiable();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(() =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new RedisConfigurationOptions()
                {
                    Servers = new List<RedisServerOptions>()
                    {
                        new()
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
        _masaConfigurationBuilder.Setup(builder => builder.Services).Returns(_services).Verifiable();
        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(() =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new RedisConfigurationOptions()
                {
                    Servers = new List<RedisServerOptions>()
                    {
                        new()
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
    public void TestUseDccAndErrorExpansionSectionOptions()
    {
        Environment.SetEnvironmentVariable(DEFAULT_ENVIRONMENT_NAME, "Test");

        _masaConfigurationBuilder.Setup(builder => builder.Services).Returns(_services).Verifiable();

        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(() =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new RedisConfigurationOptions()
                {
                    Servers = new List<RedisServerOptions>()
                    {
                        new()
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
                new()
                {
                    AppId = "Test2",
                }
            };
        }), "ConfigObjects in the extension section cannot be empty");

        _services = new ServiceCollection();
        _masaConfigurationBuilder.Setup(builder => builder.Services).Returns(_services).Verifiable();

        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(() =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new RedisConfigurationOptions()
                {
                    Servers = new List<RedisServerOptions>()
                    {
                        new()
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
                new()
                {
                    AppId = "Test2",
                    ConfigObjects = new List<string>()
                }
            };
        }), "ConfigObjects in the extension section cannot be empty");

        _services = new ServiceCollection();
        _masaConfigurationBuilder.Setup(builder => builder.Services).Returns(_services).Verifiable();

        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(() =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new RedisConfigurationOptions()
                {
                    Servers = new List<RedisServerOptions>()
                    {
                        new()
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
                new()
                {
                    AppId = "Test",
                    ConfigObjects = new List<string>()
                    {
                        "Settings"
                    }
                }
            };
        }), "The current section already exists, no need to mount repeatedly");

        _services = new ServiceCollection();
        _masaConfigurationBuilder.Setup(builder => builder.Services).Returns(_services).Verifiable();

        Assert.ThrowsException<ArgumentNullException>(() => _masaConfigurationBuilder.Object.UseDcc(() =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new RedisConfigurationOptions()
                {
                    Servers = new List<RedisServerOptions>()
                    {
                        new()
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
                new()
                {
                    AppId = "Test2",
                    ConfigObjects = new List<string>()
                    {
                        "Settings"
                    }
                },
                new()
                {
                    AppId = "Test2",
                    ConfigObjects = new List<string>()
                    {
                        "Settings"
                    }
                }
            };
        }), "The current section already exists, no need to mount repeatedly");
    }

    [DataTestMethod]
    [DataRow("Development", "Default", "WebApplication1", "Brand")]
    public void TestUseDccAndSuccess(string environment, string cluster, string appId, string configObject)
    {
        Environment.SetEnvironmentVariable(DEFAULT_ENVIRONMENT_NAME, "Test");
        var brand = new Brands("Microsoft");
        var response = JsonSerializer.Serialize(new PublishRelease()
        {
            Content = JsonSerializer.Serialize(brand),
            ConfigFormat = ConfigFormats.Json
        });
        Mock<IMemoryCacheClient> memoryCacheClient = new();
        memoryCacheClient.Setup(client => client.GetAsync(It.IsAny<string>(), It.IsAny<Action<string?>>()).Result)
            .Returns(() => response);

        var configurationApiClient = new ConfigurationApiClient(_services.BuildServiceProvider(),
            memoryCacheClient.Object, _jsonSerializerOptions, new Mock<DccSectionOptions>().Object, new List<DccSectionOptions>());
        _services.AddSingleton<IConfigurationApiClient>(configurationApiClient);

        _masaConfigurationBuilder.Object.UseDcc(() =>
        {
            return new DccConfigurationOptions()
            {
                ManageServiceAddress = "https://github.com",
                RedisOptions = new RedisConfigurationOptions()
                {
                    Servers = new List<RedisServerOptions>()
                    {
                        new()
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

    [TestMethod]
    public void TestDccConfigurationOptions()
    {
        var options = new DccConfigurationOptions
        {
            RedisOptions = new RedisConfigurationOptions()
            {
                SyncTimeout = 10
            },
            ManageServiceAddress = "https://github.com",
            SubscribeKeyPrefix = "masa.dcc.test:"
        };
        Assert.IsTrue(options.SubscribeKeyPrefix == "masa.dcc.test:");
        Assert.IsTrue(options.ManageServiceAddress == "https://github.com");
        Assert.IsTrue(options.RedisOptions.SyncTimeout == 10);
    }

    [DataTestMethod]
    [DataRow("Development", "Default", "WebApplication1", "Brand")]
    public void TestUseDccAndSingleSection(string environment, string cluster, string appId, string configObject)
    {
        CustomTrigger trigger = new CustomTrigger(_jsonSerializerOptions);
        var brand = new Brands("Microsoft");
        var response = JsonSerializer.Serialize(new PublishRelease()
        {
            Content = brand.Serialize(_jsonSerializerOptions),
            ConfigFormat = ConfigFormats.Text
        });
        Mock<IMemoryCacheClient> memoryCacheClient = new();
        memoryCacheClient.Setup(client => client.GetAsync(It.IsAny<string>(), It.IsAny<Action<string?>>()).Result)
            .Returns(() => response);
        var configurationApiClient = new ConfigurationApiClient(_services.BuildServiceProvider(),
            memoryCacheClient.Object, _jsonSerializerOptions, new Mock<DccSectionOptions>().Object, new List<DccSectionOptions>());
        _services.AddSingleton<IConfigurationApiClient>(configurationApiClient);

        _masaConfigurationBuilder.Object.UseDcc();
        Assert.IsTrue(
            configurationApiClient
                .GetRawAsync(environment, cluster, appId, configObject, It.IsAny<Action<string>>())
                .GetAwaiter()
                .GetResult().Raw == brand.Serialize(_jsonSerializerOptions));
        trigger.Execute();
    }

    [DataTestMethod]
    [DataRow("Development", "Default", "WebApplication1", "Brand")]
    public void TestUseDccAndSingleSection2(string environment, string cluster, string appId, string configObject)
    {
        CustomTrigger trigger = new CustomTrigger(_jsonSerializerOptions);
        Mock<IMemoryCacheClient> memoryCacheClient = new();
        Dictionary<string, string> masaDic = new Dictionary<string, string>()
        {
            { "Id", Guid.NewGuid().ToString() },
            { "Name", "Masa" }
        };
        var response = JsonSerializer.Serialize(new PublishRelease()
        {
            Content = masaDic.Serialize(_jsonSerializerOptions),
            ConfigFormat = ConfigFormats.Json
        });
        memoryCacheClient.Setup(client => client.GetAsync(It.IsAny<string>(), It.IsAny<Action<string?>>()).Result)
            .Returns(() => response);
        var configurationApiClient = new ConfigurationApiClient(_services.BuildServiceProvider(),
            memoryCacheClient.Object, _jsonSerializerOptions, new Mock<DccSectionOptions>().Object, new List<DccSectionOptions>());
        _services.AddSingleton<IConfigurationApiClient>(configurationApiClient);

        _masaConfigurationBuilder.Object.UseDcc();
        Assert.IsTrue(
            configurationApiClient.GetRawAsync(
                environment,
                cluster,
                appId,
                configObject,
                It.IsAny<Action<string>>()).Result.Raw == masaDic.Serialize(_jsonSerializerOptions));
    }

    [DataTestMethod]
    [DataRow("Development", "Default", "WebApplication1", "Brand")]
    public void TestUseDccAndSingleSection3(string environment, string cluster, string appId, string configObject)
    {
        Mock<IMemoryCacheClient> memoryCacheClient = new();

        var response = JsonSerializer.Serialize(new PublishRelease()
        {
            Content = "Test",
            ConfigFormat = ConfigFormats.Text
        });
        memoryCacheClient.Setup(client => client.GetAsync(It.IsAny<string>(), It.IsAny<Action<string?>>()).Result)
            .Returns(() => response);
        var configurationApiClient = new ConfigurationApiClient(_services.BuildServiceProvider(),
            memoryCacheClient.Object, _jsonSerializerOptions, new Mock<DccSectionOptions>().Object, new List<DccSectionOptions>());
        _services.AddSingleton<IConfigurationApiClient>(configurationApiClient);

        _masaConfigurationBuilder.Object.UseDcc();
        Assert.IsTrue(configurationApiClient.GetRawAsync(
            environment,
            cluster,
            appId,
            configObject,
            It.IsAny<Action<string>>()).GetAwaiter().GetResult().Raw == "Test");
    }

    [DataTestMethod]
    [DataRow("Development", "Default", "WebApplication1", "Brand")]
    public void TestUseDccAndSingleSection4(string environment, string cluster, string appId, string configObject)
    {
        Mock<IMemoryCacheClient> memoryCacheClient = new();

        var response = JsonSerializer.Serialize(new PublishRelease()
        {
            Content = null,
            ConfigFormat = ConfigFormats.Text
        });
        memoryCacheClient.Setup(client => client.GetAsync(It.IsAny<string>(), It.IsAny<Action<string?>>()).Result)
            .Returns(() => response);
        var configurationApiClient = new ConfigurationApiClient(_services.BuildServiceProvider(),
            memoryCacheClient.Object, _jsonSerializerOptions, new Mock<DccSectionOptions>().Object, new List<DccSectionOptions>());
        _services.AddSingleton<IConfigurationApiClient>(configurationApiClient);

        _masaConfigurationBuilder.Object.UseDcc();
        Assert.IsTrue(configurationApiClient.GetRawAsync(
            environment,
            cluster,
            appId,
            configObject,
            It.IsAny<Action<string>>()).GetAwaiter().GetResult().Raw == null);
    }

    [TestMethod]
    public void TestUseDccAndSingleSection5()
    {
        CustomTrigger trigger = new CustomTrigger(_jsonSerializerOptions);
        Mock<IMemoryCacheClient> memoryCacheClient = new();
        var response = JsonSerializer.Serialize(new PublishRelease()
        {
            Content = "Test",
            ConfigFormat = (ConfigFormats)4
        });
        memoryCacheClient.Setup(client => client.GetAsync(It.IsAny<string>(), It.IsAny<Action<string?>>()).Result)
            .Returns(() => response);
        var configurationApiClient = new ConfigurationApiClient(_services.BuildServiceProvider(),
            memoryCacheClient.Object, _jsonSerializerOptions, new Mock<DccSectionOptions>().Object, new List<DccSectionOptions>());
        _services.AddSingleton<IConfigurationApiClient>(configurationApiClient);

        Assert.ThrowsException<NotSupportedException>(() => _masaConfigurationBuilder.Object.UseDcc());
    }

    [DataTestMethod]
    [DataRow("Development", "Default", "WebApplication1", "Brand")]
    public void TestUseMultiDcc(string environment, string cluster, string appId, string configObject)
    {
        var brand = new Brands("Microsoft");
        var response = JsonSerializer.Serialize(new PublishRelease()
        {
            Content = JsonSerializer.Serialize(brand),
            ConfigFormat = ConfigFormats.Json
        });
        Mock<IMemoryCacheClient> memoryCacheClient = new();
        memoryCacheClient.Setup(client => client.GetAsync(It.IsAny<string>(), It.IsAny<Action<string?>>()).Result).Returns(() => response);

        var configurationApiClient = new ConfigurationApiClient(_services.BuildServiceProvider(), memoryCacheClient.Object,
            _jsonSerializerOptions, new Mock<DccSectionOptions>().Object, new List<DccSectionOptions>());
        _services.AddSingleton<IConfigurationApiClient>(configurationApiClient);

        _masaConfigurationBuilder.Object.UseDcc().UseDcc();
        var result = configurationApiClient.GetRawAsync(
            environment,
            cluster,
            appId,
            configObject,
            It.IsAny<Action<string>>()).ConfigureAwait(false).GetAwaiter().GetResult();
        Assert.IsTrue(result.Raw == JsonSerializer.Serialize(brand));

        var httpClient = _services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient(DEFAULT_CLIENT_NAME);
        Assert.IsTrue(httpClient.BaseAddress!.ToString() == "http://localhost:6379/");
    }

    [TestMethod]
    public void TestLoadPropertiesShouldReturnJson()
    {
        var brands = new Brands("Microsoft");
        Mock<IConfigurationApiClient> configurationClient = new();
        configurationClient.Setup(client => client.GetRawAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<Action<string>>())).ReturnsAsync((JsonSerializer.Serialize(brands), ConfigurationTypes.Json));
    }

    [TestMethod]
    public void TestMasaConfigurationByConfigurationAPIReturnKeyIsExist()
    {
        var builder = WebApplication.CreateBuilder();
        var brand = new Brands("Apple");
        builder.Services.AddMemoryCache();
        string key = "Development-Default-WebApplication1-Brand".ToLower();
        builder.Services.AddSingleton<IMemoryCacheClientFactory>(serviceProvider =>
        {
            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
            string value = new PublishRelease()
            {
                Content = brand.Serialize(_jsonSerializerOptions),
                ConfigFormat = ConfigFormats.Json
            }.Serialize(_jsonSerializerOptions);
            memoryCache.Set($"{key}String", value);
            return new CustomMemoryCacheClientFactory(memoryCache);
        });

        builder.AddMasaConfiguration(masaBuilder => masaBuilder.UseDcc());

        var serviceProvider = builder.Services.BuildServiceProvider();
        var masaConfiguration = serviceProvider.GetService<IMasaConfiguration>();
        Assert.IsTrue(masaConfiguration != null);
        var configuration = masaConfiguration!.ConfigurationApi;
        Assert.IsNotNull(configuration);

        Assert.IsTrue(configuration.Get("WebApplication1")["Brand:Name"] == "Apple");
    }

    [TestMethod]
    public void TestGetSecretRenturnSecretEqualSecret()
    {
        var builder = WebApplication.CreateBuilder();
        var brand = new Brands("Apple");
        builder.Services.AddMemoryCache();
        string key = "Development-Default-WebApplication1-Brand".ToLower();
        builder.Services.AddSingleton<IMemoryCacheClientFactory>(serviceProvider =>
        {
            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
            string value = new PublishRelease()
            {
                Content = brand.Serialize(_jsonSerializerOptions),
                ConfigFormat = ConfigFormats.Json
            }.Serialize(_jsonSerializerOptions);
            memoryCache.Set($"{key}String", value);
            return new CustomMemoryCacheClientFactory(memoryCache);
        });
        builder.AddMasaConfiguration(configurationBuilder => configurationBuilder.UseDcc());

        var serviceProvider = builder.Services.BuildServiceProvider();
        var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();
        var field = typeof(ConfigurationApiBase).GetField("_defaultSectionOption", BindingFlags.Instance | BindingFlags.NonPublic);
        var option = field!.GetValue(configurationApiClient);
        Assert.IsTrue(((DccSectionOptions)option!).Secret == "Secret");
    }
}
