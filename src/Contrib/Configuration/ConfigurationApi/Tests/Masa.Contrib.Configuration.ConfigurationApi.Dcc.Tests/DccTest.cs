// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests;

[TestClass]
public class DccTest
{
    private const string DEFAULT_CLIENT_NAME = "masa.contrib.configuration.configurationapi.dcc";
    private Mock<IMasaConfigurationBuilder> _masaConfigurationBuilder;
    private JsonSerializerOptions _jsonSerializerOptions;
    private IServiceCollection _services;

    private Mock<IMultilevelCacheClientFactory> _memoryCacheClientFactory;
    private Mock<IDistributedCacheClientFactory> _distributedCacheClientFactory;
    private Mock<IMemoryCache> _memoryCache;
    private Mock<IManualDistributedCacheClient> _distributedCacheClient;
    private const string DEFAULT_PUBLIC_ID = "public-$Config";
    private Masa.BuildingBlocks.Data.ISerializer _serializer;
    private Masa.BuildingBlocks.Data.IDeserializer _deserializer;

#pragma warning disable CS0618
    [TestInitialize]
    public void Initialize()
    {
        var builder = WebApplication.CreateBuilder();
        builder = builder.InitializeAppConfiguration();
        _services = builder.Services;
        _masaConfigurationBuilder = new();
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build;
        _masaConfigurationBuilder.Setup(masaConfigurationBuilder => masaConfigurationBuilder.Configuration).Returns(configuration)
            .Verifiable();
        _masaConfigurationBuilder.Setup(masaConfigurationBuilder => masaConfigurationBuilder.Services).Returns(_services).Verifiable();
        _memoryCacheClientFactory = new();
        _distributedCacheClientFactory = new();
        _memoryCache = new();
        _distributedCacheClient = new();
        _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        _serializer = new DefaultYamlSerializer(new SerializerBuilder().JsonCompatible().Build());
        _deserializer = new DefaultYamlDeserializer(new DeserializerBuilder().Build());

        var serializerFactory = new Mock<ISerializerFactory>();
        var deserializerFactory = new Mock<IDeserializerFactory>();
        serializerFactory.Setup(factory => factory.Create(DEFAULT_CLIENT_NAME)).Returns(() => _serializer);
        deserializerFactory.Setup(factory => factory.Create(DEFAULT_CLIENT_NAME)).Returns(() => _deserializer);

        _services.AddSingleton(_ => serializerFactory.Object);
        _services.AddSingleton(_ => deserializerFactory.Object);
    }
#pragma warning restore CS0618

    [TestMethod]
    public void TestTryAddConfigurationApiClient()
    {
        _memoryCacheClientFactory.Setup(factory => factory.Create(DEFAULT_CLIENT_NAME)).Returns(() => null!).Verifiable();
        _services.AddSingleton(_ => _memoryCacheClientFactory.Object);
        MasaConfigurationExtensions.TryAddConfigurationApiClient(_services, new DccOptions(), new DccSectionOptions(),
            new List<DccSectionOptions>(), null!);
        Assert.IsTrue(_services.Count(service
            => service.ServiceType == typeof(IConfigurationApiClient) && service.Lifetime == ServiceLifetime.Singleton) == 1);
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            var _ = _services.BuildServiceProvider().GetServices<IConfigurationApiClient>();
        });

        _services = new ServiceCollection();
        _memoryCacheClientFactory
            .Setup(factory => factory.Create(DEFAULT_CLIENT_NAME))
            .Returns(() => new MultilevelCacheClient(
                _memoryCache.Object,
                _distributedCacheClient.Object,
                new MultilevelCacheOptions()
                {
                    CacheKeyType = new CacheOptions().CacheKeyType
                },
                SubscribeKeyType.ValueTypeFullNameAndKey
            ))
            .Verifiable();
        _services.AddSingleton(_ => _memoryCacheClientFactory.Object);
        MasaConfigurationExtensions.TryAddConfigurationApiClient(_services, new DccOptions(), new DccSectionOptions(),
            new List<DccSectionOptions>(),
            new JsonSerializerOptions()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

        var clienties = _services.BuildServiceProvider().GetServices<IConfigurationApiClient>();
        Assert.IsTrue(clienties.Count() == 1);

        _services = new ServiceCollection();
        _memoryCacheClientFactory
            .Setup(factory => factory.Create(DEFAULT_CLIENT_NAME))
            .Returns(() => new MultilevelCacheClient(_memoryCache.Object,
                _distributedCacheClient.Object,
                new MultilevelCacheOptions()
                {
                    CacheKeyType = new CacheOptions().CacheKeyType
                },
                SubscribeKeyType.ValueTypeFullNameAndKey))
            .Verifiable();
        _services.AddSingleton(_ => _memoryCacheClientFactory.Object);
        MasaConfigurationExtensions.TryAddConfigurationApiClient(_services, new DccOptions(), new DccSectionOptions(),
            new List<DccSectionOptions>(),
            _jsonSerializerOptions);
        MasaConfigurationExtensions.TryAddConfigurationApiClient(_services, new DccOptions(), new DccSectionOptions(),
            new List<DccSectionOptions>(),
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
    public void TestCustomCaller()
    {
        var response = new PublishReleaseModel()
        {
            Content = string.Empty,
            ConfigFormat = ConfigFormats.Raw
        };
        Mock<IManualMultilevelCacheClient> memoryCacheClient = new();
        memoryCacheClient
            .Setup(client => client.GetAsync(It.IsAny<string>(), It.IsAny<Action<PublishReleaseModel?>>(), null).Result)
            .Returns(() => response);

        Mock<IMultilevelCacheClientFactory> memoryCacheClientFactory = new();
        memoryCacheClientFactory
            .Setup(factory => factory.Create(DEFAULT_CLIENT_NAME))
            .Returns(() => memoryCacheClient.Object);
        _services.AddSingleton(_ => memoryCacheClientFactory.Object);

        var configurationApiClient = new ConfigurationApiClient(
            _services.BuildServiceProvider(),
            _jsonSerializerOptions,
            new Mock<DccOptions>().Object,
            new Mock<DccSectionOptions>().Object,
            new List<DccSectionOptions>());
        _services.AddSingleton<IConfigurationApiClient>(configurationApiClient);
        _masaConfigurationBuilder.Object.UseDcc(new DccOptions()
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
            },
            AppId = "Test",
            Environment = "Test",
            ConfigObjects = new List<string>()
            {
                "Settings"
            }
        }, jsonSerializerOption =>
        {
            jsonSerializerOption.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        }, option =>
        {
            option.UseHttpClient(builder =>
            {
                builder.Configure = opt => opt.BaseAddress = new Uri("https://github.com");
            });
        });
        var caller = _services.BuildServiceProvider().GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
        Assert.IsNotNull(caller);
    }

    [TestMethod]
    public void TestUseMultiDcc()
    {
        var brand = new Brands("Microsoft");
        var response = new PublishReleaseModel()
        {
            Content = JsonSerializer.Serialize(brand),
            ConfigFormat = ConfigFormats.Json
        };
        Mock<IManualMultilevelCacheClient> memoryCacheClient = new();
        memoryCacheClient
            .Setup(client => client.GetAsync(It.IsAny<string>(), It.IsAny<Action<PublishReleaseModel?>>(), null).Result)
            .Returns(() => response);
        Mock<IMultilevelCacheClientFactory> memoryCacheClientFactory = new();
        memoryCacheClientFactory
            .Setup(factory => factory.Create(DEFAULT_CLIENT_NAME))
            .Returns(() => memoryCacheClient.Object);
        _services.AddSingleton(_ => memoryCacheClientFactory.Object);
        _masaConfigurationBuilder.Object.UseDcc();
        var serviceCount = _masaConfigurationBuilder.Object.Services.Count;
        _masaConfigurationBuilder.Object.UseDcc();
        Assert.AreEqual(serviceCount, _masaConfigurationBuilder.Object.Services.Count);
    }

#pragma warning disable CS0618
    [TestMethod]
    public void TestMasaConfigurationByConfigurationApiReturnKeyIsExist()
    {
        var builder = WebApplication.CreateBuilder();
        var brand = new Brands("Apple");
        // builder.Services.AddMemoryCache();
        builder.Services.AddStackExchangeRedisCache(DEFAULT_CLIENT_NAME, new RedisConfigurationOptions()
        {
            Servers = new List<RedisServerOptions>()
            {
                new("localhost", 6379)
            }
        }).AddMultilevelCache();
        string key = "Development-Default-WebApplication1-Brand".ToLower();
        var serviceProvider = builder.Services.BuildServiceProvider();
        var multilevelCacheClient = serviceProvider.GetRequiredService<IMultilevelCacheClient>();
        var value = new PublishReleaseModel()
        {
            Content = brand.Serialize(_jsonSerializerOptions),
            ConfigFormat = ConfigFormats.Json
        };
        multilevelCacheClient.Set(key, value);

        builder.AddMasaConfiguration(masaBuilder => masaBuilder.UseDcc());

        serviceProvider = builder.Services.BuildServiceProvider();
        var masaConfiguration = serviceProvider.GetService<IMasaConfiguration>();
        Assert.IsTrue(masaConfiguration != null);
        var configuration = masaConfiguration!.ConfigurationApi;
        Assert.IsNotNull(configuration);

        Assert.IsTrue(configuration.Get("WebApplication1")["Brand:Name"] == "Apple");
    }

    [TestMethod]
    public void TestGetSecretReturnSecretEqualSecret()
    {
        var builder = WebApplication.CreateBuilder();
        var brand = new Brands("Apple");
        string key = "Development-Default-WebApplication1-Brand".ToLower();
        builder.Services.AddMultilevelCache(
            DEFAULT_CLIENT_NAME,
            distributedCacheOptions => distributedCacheOptions.UseStackExchangeRedisCache(new RedisConfigurationOptions()
            {
                Servers = new List<RedisServerOptions>()
                {
                    new("localhost", 6379)
                }
            }),
            multilevelCacheOptions =>
            {
                multilevelCacheOptions.GlobalCacheOptions = new CacheOptions()
                {
                    CacheKeyType = CacheKeyType.None
                };
            }
        );
        var serviceProvider = builder.Services.BuildServiceProvider();
        var multilevelCacheClient = serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>().Create(DEFAULT_CLIENT_NAME);
        string value = new PublishReleaseModel()
        {
            Content = brand.Serialize(_jsonSerializerOptions),
            ConfigFormat = ConfigFormats.Json
        }.Serialize(_jsonSerializerOptions);
        multilevelCacheClient.Set(key, value);

        builder.AddMasaConfiguration(configurationBuilder => configurationBuilder.UseDcc());

        serviceProvider = builder.Services.BuildServiceProvider();
        var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();
        var field = typeof(ConfigurationApiBase).GetField("_defaultSectionOption", BindingFlags.Instance | BindingFlags.NonPublic);
        var option = field!.GetValue(configurationApiClient);
        Assert.IsTrue(((DccSectionOptions)option!).Secret == "Secret");
    }
#pragma warning restore CS0618

    [TestMethod]
    public void TestTypeConversionByDccOptions()
    {
        DccOptions dccOptions = new DccOptions()
        {
            RedisOptions = new RedisConfigurationOptions()
            {
                Servers = new List<RedisServerOptions>()
                {
                    new("localhost", 6379),
                    new("localhost", 6378)
                },
                AbortOnConnectFail = true,
                AllowAdmin = true,
                ClientName = nameof(DccOptions.RedisOptions.ClientName),
                ChannelPrefix = nameof(DccOptions.RedisOptions.ChannelPrefix),
                ConnectRetry = 1,
                ConnectTimeout = 300,
                DefaultDatabase = 1,
                Password = nameof(DccOptions.RedisOptions.Password),
                Proxy = StackExchange.Redis.Proxy.Twemproxy,
                Ssl = true,
                SyncTimeout = 3000,
                AbsoluteExpiration = DateTimeOffset.Now.AddHours(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromHours(2),
            },
            ManageServiceAddress = nameof(DccOptions.ManageServiceAddress),
            SubscribeKeyPrefix = nameof(DccOptions.SubscribeKeyPrefix),
            PublicId = nameof(DccOptions.PublicId),
            PublicSecret = nameof(DccOptions.PublicSecret),
            AppId = "appid",
            Environment = "test",
            Cluster = "default",
            ConfigObjects = new List<string>()
            {
                "configObjects"
            },
            Secret = "secret",
            ExpandSections = new List<DccSectionOptions>()
            {
                new("appid2", "dev", "default2", new List<string> { "configObjects2" }, "secret2")
            }
        };
        DccConfigurationOptions dccConfigurationOptions = dccOptions;
        Assert.AreEqual(dccOptions.ManageServiceAddress, dccConfigurationOptions.ManageServiceAddress);
        Assert.AreEqual(dccOptions.SubscribeKeyPrefix, dccConfigurationOptions.SubscribeKeyPrefix);
        Assert.AreEqual(dccOptions.AppId, dccConfigurationOptions.DefaultSection.AppId);
        Assert.AreEqual(dccOptions.Environment, dccConfigurationOptions.DefaultSection.Environment);
        Assert.AreEqual(dccOptions.Cluster, dccConfigurationOptions.DefaultSection.Cluster);
        Assert.AreEqual(dccOptions.ConfigObjects.Count, dccConfigurationOptions.DefaultSection.ConfigObjects.Count);
        Assert.AreEqual(dccOptions.ConfigObjects[0], dccConfigurationOptions.DefaultSection.ConfigObjects[0]);
        Assert.AreEqual(dccOptions.Secret, dccConfigurationOptions.DefaultSection.Secret);
        Assert.AreEqual(dccOptions.ExpandSections.Count, dccConfigurationOptions.ExpandSections.Count);
        Assert.AreEqual(dccOptions.ExpandSections[0].AppId, dccConfigurationOptions.ExpandSections[0].AppId);
        Assert.AreEqual(dccOptions.ExpandSections[0].Environment, dccConfigurationOptions.ExpandSections[0].Environment);
        Assert.AreEqual(dccOptions.ExpandSections[0].Cluster, dccConfigurationOptions.ExpandSections[0].Cluster);
        Assert.AreEqual(dccOptions.ExpandSections[0].Secret, dccConfigurationOptions.ExpandSections[0].Secret);
        Assert.AreEqual(dccOptions.ExpandSections[0].ConfigObjects.Count,
            dccConfigurationOptions.ExpandSections[0].ConfigObjects.Count);
        Assert.AreEqual(dccOptions.ExpandSections[0].ConfigObjects[0], dccConfigurationOptions.ExpandSections[0].ConfigObjects[0]);
        Assert.AreEqual(dccOptions.RedisOptions.AbortOnConnectFail, dccConfigurationOptions.RedisOptions.AbortOnConnectFail);
        Assert.AreEqual(dccOptions.RedisOptions.AllowAdmin, dccConfigurationOptions.RedisOptions.AllowAdmin);
        Assert.AreEqual(dccOptions.RedisOptions.ClientName, dccConfigurationOptions.RedisOptions.ClientName);
        Assert.AreEqual(dccOptions.RedisOptions.ChannelPrefix, dccConfigurationOptions.RedisOptions.ChannelPrefix);
        Assert.AreEqual(dccOptions.RedisOptions.ConnectRetry, dccConfigurationOptions.RedisOptions.ConnectRetry);
        Assert.AreEqual(dccOptions.RedisOptions.ConnectTimeout, dccConfigurationOptions.RedisOptions.ConnectTimeout);
        Assert.AreEqual(dccOptions.RedisOptions.DefaultDatabase, dccConfigurationOptions.RedisOptions.DefaultDatabase);
        Assert.AreEqual(dccOptions.RedisOptions.Password, dccConfigurationOptions.RedisOptions.Password);
        Assert.AreEqual(dccOptions.RedisOptions.Proxy, dccConfigurationOptions.RedisOptions.Proxy);
        Assert.AreEqual(dccOptions.RedisOptions.Ssl, dccConfigurationOptions.RedisOptions.Ssl);
        Assert.AreEqual(dccOptions.RedisOptions.SyncTimeout, dccConfigurationOptions.RedisOptions.SyncTimeout);
        Assert.AreEqual(dccOptions.RedisOptions.AbsoluteExpiration, dccConfigurationOptions.RedisOptions.AbsoluteExpiration);
        Assert.AreEqual(dccOptions.RedisOptions.AbsoluteExpirationRelativeToNow,
            dccConfigurationOptions.RedisOptions.AbsoluteExpirationRelativeToNow);
        Assert.AreEqual(dccOptions.RedisOptions.SlidingExpiration, dccConfigurationOptions.RedisOptions.SlidingExpiration);
        Assert.AreEqual(dccOptions.RedisOptions.Servers.Count, dccConfigurationOptions.RedisOptions.Servers.Count);
        Assert.AreEqual(dccOptions.RedisOptions.Servers[0].Host, dccConfigurationOptions.RedisOptions.Servers[0].Host);
        Assert.AreEqual(dccOptions.RedisOptions.Servers[0].Port, dccConfigurationOptions.RedisOptions.Servers[0].Port);
        Assert.AreEqual(dccOptions.RedisOptions.Servers[1].Host, dccConfigurationOptions.RedisOptions.Servers[1].Host);
        Assert.AreEqual(dccOptions.RedisOptions.Servers[1].Port, dccConfigurationOptions.RedisOptions.Servers[1].Port);
    }

    [TestMethod]
    public void TestComplementAndCheckDccConfigurationOption()
    {
        string appid = "test";
        string environment = "dev";
        string cluster = "default";
        _services.Configure<MasaAppConfigureOptions>(options =>
        {
            options.AppId = appid;
            options.Environment = environment;
            options.Cluster = cluster;
        });
        var configObjects = new List<string>()
        {
            "configObject",
            "configObject2"
        };
        var publicConfigObjects = new List<string>()
        {
            "configObject3",
        };
        MockDistributedCacheClient(appid, environment, cluster, configObjects);
        MockDistributedCacheClient(DEFAULT_PUBLIC_ID, environment, cluster, publicConfigObjects);
        MockDistributedCacheClientFactory();

        DccOptions dccOptions = new DccOptions()
        {
            ManageServiceAddress = nameof(DccOptions.ManageServiceAddress),
            RedisOptions = new RedisConfigurationOptions()
            {
                Servers = new List<RedisServerOptions>()
                {
                    new()
                }
            }
        };
        var dccConfigurationOptions =
            MasaConfigurationExtensions.ComplementAndCheckDccConfigurationOption(_masaConfigurationBuilder.Object, dccOptions);

        Assert.IsNotNull(dccConfigurationOptions.DefaultSection);
        Assert.AreEqual(appid, dccConfigurationOptions.DefaultSection.AppId);
        Assert.AreEqual(environment, dccConfigurationOptions.DefaultSection.Environment);
        Assert.AreEqual(cluster, dccConfigurationOptions.DefaultSection.Cluster);
        Assert.AreEqual(string.Empty, dccConfigurationOptions.DefaultSection.Secret);
        Assert.AreEqual(configObjects.Count, dccConfigurationOptions.DefaultSection.ConfigObjects.Count);
        Assert.AreEqual(configObjects[0], dccConfigurationOptions.DefaultSection.ConfigObjects[0]);
        Assert.AreEqual(configObjects[1], dccConfigurationOptions.DefaultSection.ConfigObjects[1]);

        Assert.AreEqual(1, dccConfigurationOptions.ExpandSections.Count);
        Assert.AreEqual(DEFAULT_PUBLIC_ID, dccConfigurationOptions.ExpandSections[0].AppId);
        Assert.AreEqual(environment, dccConfigurationOptions.ExpandSections[0].Environment);
        Assert.AreEqual(cluster, dccConfigurationOptions.ExpandSections[0].Cluster);
        Assert.AreEqual(string.Empty, dccConfigurationOptions.ExpandSections[0].Secret);
        Assert.AreEqual(publicConfigObjects.Count, dccConfigurationOptions.ExpandSections[0].ConfigObjects.Count);
        Assert.AreEqual(publicConfigObjects[0], dccConfigurationOptions.ExpandSections[0].ConfigObjects[0]);
        Assert.AreEqual(2, dccConfigurationOptions.GetAllSections().Count());
    }

    [TestMethod]
    public void TestComplementAndCheckDccConfigurationOptionByCustomDccOptions()
    {
        string appid = "test";
        string environment = "dev";
        string cluster = "default";
        _services.Configure<MasaAppConfigureOptions>(options =>
        {
            options.AppId = appid;
            options.Environment = environment;
            options.Cluster = cluster;
        });
        var configObjects = new List<string>()
        {
            "configObject",
            "configObject2"
        };
        var publicConfigObjects = new List<string>()
        {
            "configObject3",
        };

        string customAppid = "custom-test";
        string customEnvironment = "custom-dev";
        string customCluster = "custom-default";
        string customPublic = "custom-public";
        string customPublicSecret = "custom-public-secret";

        MockDistributedCacheClient(customAppid, customEnvironment, customCluster, configObjects);
        MockDistributedCacheClient(customPublic, customEnvironment, customCluster, publicConfigObjects);
        MockDistributedCacheClientFactory();

        DccOptions dccOptions = new DccOptions()
        {
            ManageServiceAddress = nameof(DccOptions.ManageServiceAddress),
            RedisOptions = new RedisConfigurationOptions()
            {
                Servers = new List<RedisServerOptions>()
                {
                    new()
                }
            },
            AppId = customAppid,
            Environment = customEnvironment,
            Cluster = customCluster,
            PublicId = customPublic,
            PublicSecret = customPublicSecret
        };
        var dccConfigurationOptions =
            MasaConfigurationExtensions.ComplementAndCheckDccConfigurationOption(_masaConfigurationBuilder.Object, dccOptions);

        Assert.IsNotNull(dccConfigurationOptions.DefaultSection);
        Assert.AreEqual(customAppid, dccConfigurationOptions.DefaultSection.AppId);
        Assert.AreEqual(customEnvironment, dccConfigurationOptions.DefaultSection.Environment);
        Assert.AreEqual(customCluster, dccConfigurationOptions.DefaultSection.Cluster);
        Assert.AreEqual(string.Empty, dccConfigurationOptions.DefaultSection.Secret);
        Assert.AreEqual(configObjects.Count, dccConfigurationOptions.DefaultSection.ConfigObjects.Count);
        Assert.AreEqual(configObjects[0], dccConfigurationOptions.DefaultSection.ConfigObjects[0]);
        Assert.AreEqual(configObjects[1], dccConfigurationOptions.DefaultSection.ConfigObjects[1]);

        Assert.AreEqual(1, dccConfigurationOptions.ExpandSections.Count);
        Assert.AreEqual(customPublic, dccConfigurationOptions.ExpandSections[0].AppId);
        Assert.AreEqual(customEnvironment, dccConfigurationOptions.ExpandSections[0].Environment);
        Assert.AreEqual(customCluster, dccConfigurationOptions.ExpandSections[0].Cluster);
        Assert.AreEqual(customPublicSecret, dccConfigurationOptions.ExpandSections[0].Secret);
        Assert.AreEqual(publicConfigObjects.Count, dccConfigurationOptions.ExpandSections[0].ConfigObjects.Count);
        Assert.AreEqual(publicConfigObjects[0], dccConfigurationOptions.ExpandSections[0].ConfigObjects[0]);
        Assert.AreEqual(2, dccConfigurationOptions.GetAllSections().Count());
    }

    [TestMethod]
    public void TestComplementAndCheckDccConfigurationOptionByManageServiceAddressIsEmpty()
    {
        DccOptions dccOptions = new DccOptions();
        Assert.ThrowsException<MasaArgumentException>(() =>
        {
            MasaConfigurationExtensions.ComplementAndCheckDccConfigurationOption(_masaConfigurationBuilder.Object, dccOptions);
        });
    }

    [TestMethod]
    public void TestComplementAndCheckDccConfigurationOptionByRedisServersIsEmpty()
    {
        DccOptions dccOptions = new DccOptions()
        {
            ManageServiceAddress = nameof(DccOptions.ManageServiceAddress),
        };
        Assert.ThrowsException<MasaArgumentException>(() =>
        {
            MasaConfigurationExtensions.ComplementAndCheckDccConfigurationOption(_masaConfigurationBuilder.Object, dccOptions);
        });
    }

    [TestMethod]
    public void TestComplementAndCheckDccConfigurationOptionByRepeatAppId()
    {
        DccOptions dccOptions = new DccOptions()
        {
            ManageServiceAddress = nameof(DccOptions.ManageServiceAddress),
            RedisOptions = new RedisConfigurationOptions()
            {
                Servers = new List<RedisServerOptions>()
                {
                    new()
                }
            },
            ExpandSections = new List<DccSectionOptions>()
            {
                new()
                {
                    AppId = "test1"
                },
                new()
                {
                    AppId = "test1"
                }
            }
        };
        Assert.ThrowsException<ArgumentException>(() =>
        {
            MasaConfigurationExtensions.ComplementAndCheckDccConfigurationOption(_masaConfigurationBuilder.Object, dccOptions);
        });
    }

    [TestMethod]
    public void TestComplementAndCheckDccConfigurationOptionByAppIdIsEmpty()
    {
        DccOptions dccOptions = new DccOptions()
        {
            ManageServiceAddress = nameof(DccOptions.ManageServiceAddress),
            RedisOptions = new RedisConfigurationOptions()
            {
                Servers = new List<RedisServerOptions>()
                {
                    new()
                }
            },
            ExpandSections = new List<DccSectionOptions>()
            {
                new()
                {
                    AppId = ""
                },
                new()
                {
                    AppId = "test1"
                }
            }
        };
        Assert.ThrowsException<MasaArgumentException>(() =>
        {
            MasaConfigurationExtensions.ComplementAndCheckDccConfigurationOption(_masaConfigurationBuilder.Object, dccOptions);
        });
    }

    private void MockDistributedCacheClientFactory()
    {
        _distributedCacheClientFactory
            .Setup(factory => factory.Create(DEFAULT_CLIENT_NAME))
            .Returns(() => _distributedCacheClient.Object)
            .Verifiable();
        _services.AddSingleton(_ => _distributedCacheClientFactory.Object);
    }

    private void MockDistributedCacheClient(
        string appId,
        string environment,
        string cluster,
        List<string> mockKeys)
    {
        string partialKey =
            $"{environment}-{cluster}-{appId}".ToLower();
        _distributedCacheClient.Setup(client => client.GetKeys<PublishReleaseModel>($"{partialKey}*", null)).Returns(mockKeys);
    }
}
