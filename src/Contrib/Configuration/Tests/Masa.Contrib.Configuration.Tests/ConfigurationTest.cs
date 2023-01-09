// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.Tests;

#pragma warning disable CS0618
[TestClass]
public class ConfigurationTest
{
    [TestMethod]
    public void TestDefaultMasaConfiguration()
    {
        var configurationApi = Mock.Of<IConfigurationApi>();

        var builder = WebApplication.CreateBuilder();
        var masaConfiguration = new DefaultMasaConfiguration(builder.Configuration, configurationApi);
        var localConfiguration = masaConfiguration.GetConfiguration(SectionTypes.Local);
        var configurationApiConfiguration = masaConfiguration.GetConfiguration(SectionTypes.Local);
        Assert.IsTrue(!((IConfigurationSection)localConfiguration).Exists());
        Assert.IsTrue(!((IConfigurationSection)configurationApiConfiguration).Exists());
    }

    [TestMethod]
    public void TestAddMasaConfigurationShouldThrowException()
    {
        var builder = WebApplication.CreateBuilder();
        Assert.ThrowsException<MasaException>(() => builder.AddMasaConfiguration());
    }

    [TestMethod]
    public void TestAddJsonFileShouldReturnRabbitMqOptionAndSystemOptionsExist()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddMasaConfiguration(masaConfigurationBuilder
            => masaConfigurationBuilder.AddJsonFile("rabbitMq.json", optional: false, reloadOnChange: true));
        var configurationApi = Mock.Of<IConfigurationApi>();
        var masaConfiguration = new DefaultMasaConfiguration(builder.Configuration, configurationApi);
        var localConfiguration = masaConfiguration.GetConfiguration(SectionTypes.Local);
        var configurationApiConfiguration = masaConfiguration.GetConfiguration(SectionTypes.ConfigurationApi);
        Assert.IsTrue(((IConfigurationSection)localConfiguration).Exists());
        Assert.IsTrue(!((IConfigurationSection)configurationApiConfiguration).Exists());

        var serviceProvider = builder.Services.BuildServiceProvider();
        var rabbitMqOptions = serviceProvider.GetRequiredService<IOptions<RabbitMqOptions>>();
        Assert.IsTrue(rabbitMqOptions is
            { Value.HostName: "localhost", Value.UserName: "admin", Value.Password: "admin", Value.VirtualHost: "/", Value.Port: "5672" });

        var systemOptions = serviceProvider.GetRequiredService<IOptions<SystemOptions>>();
        Assert.IsTrue(systemOptions is { Value.Name: "Masa TEST" });

        var redisOptions = serviceProvider.GetRequiredService<IOptions<RedisOptions>>();
        Assert.IsTrue(redisOptions is { Value.Ip: null, Value.Password: null, Value.Port: 0 });
    }

    [TestMethod]
    public void TestManuallyMappingShouldReturnRedisExist()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddMasaConfiguration(masaConfigurationBuilder =>
        {
            masaConfigurationBuilder
                .AddJsonFile("rabbitMq.json", optional: false, reloadOnChange: true)
                .AddJsonFile("redis.json", optional: false, reloadOnChange: true);

            masaConfigurationBuilder.UseMasaOptions(options =>
            {
                options.MappingLocal<RedisOptions>();
            });
        });

        var configurationApi = Mock.Of<IConfigurationApi>();
        var masaConfiguration = new DefaultMasaConfiguration(builder.Configuration, configurationApi);
        var localConfiguration = masaConfiguration.GetConfiguration(SectionTypes.Local);
        var configurationApiConfiguration = masaConfiguration.GetConfiguration(SectionTypes.ConfigurationApi);
        Assert.IsTrue(((IConfigurationSection)localConfiguration).Exists());
        Assert.IsTrue(!((IConfigurationSection)configurationApiConfiguration).Exists());

        var serviceProvider = builder.Services.BuildServiceProvider();
        var rabbitMqOptions = serviceProvider.GetRequiredService<IOptions<RabbitMqOptions>>();
        Assert.IsTrue(rabbitMqOptions is
            { Value.HostName: "localhost", Value.UserName: "admin", Value.Password: "admin", Value.VirtualHost: "/", Value.Port: "5672" });

        var systemOptions = serviceProvider.GetRequiredService<IOptions<SystemOptions>>();
        Assert.IsTrue(systemOptions is { Value.Name: "Masa TEST" });

        var redisOptions = serviceProvider.GetRequiredService<IOptions<RedisOptions>>();
        Assert.IsTrue(redisOptions is { Value.Ip: "localhost", Value.Password: "", Value.Port: 6379 });
    }

    [TestMethod]
    public void TestAddMultiMasaConfigurationShouldReturnIMasaConfigurationCount1()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddMasaConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddJsonFile("redis.json", true, true)
                .AddJsonFile("rabbitMq.json", true, true);

            configurationBuilder.UseMasaOptions(option => option.MappingLocal<RedisOptions>("RedisOptions"));
        }).AddMasaConfiguration();
        var serviceProvider = builder.Services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var redisOption = serviceProvider.GetRequiredService<IOptions<RedisOptions>>();
        Assert.IsTrue(configuration["Local:RedisOptions:Ip"] == "localhost");
        Assert.IsTrue(redisOption.Value.Ip == "localhost");

        var rabbitMqOption = serviceProvider.GetRequiredService<IOptions<RabbitMqOptions>>();
        Assert.IsTrue(configuration["Local:RabbitMq:UserName"] == "admin");
        Assert.IsTrue(rabbitMqOption.Value.UserName == "admin" && rabbitMqOption.Value.Password == "admin");

        Assert.IsTrue(serviceProvider.GetServices<IMasaConfiguration>().Count() == 1);
    }

    [TestMethod]
    public void TestAutoMapSectionErrorShouldThrowException()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Host.ConfigureAppConfiguration((_, config) => config.Sources.Clear());
        var chainedConfiguration = new ConfigurationBuilder()
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true);
        builder.Configuration.AddConfiguration(chainedConfiguration.Build());

        Assert.ThrowsException<MasaException>(()
                => builder.AddMasaConfiguration()
            , $"Check if the mapping section is correct，section name is [{It.IsAny<string>()}]");
    }

    [TestMethod]
    public void TestSpecifyAssembliesShouldKafKaOptionsExist()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Host.ConfigureAppConfiguration((_, config) => config.Sources.Clear());
        var chainedConfiguration = new ConfigurationBuilder()
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true);
        builder.Configuration.AddConfiguration(chainedConfiguration.Build());
        builder.AddMasaConfiguration(new List<Assembly>()
        {
            typeof(KafkaOptions).Assembly
        });

        var serviceProvider = builder.Services.BuildServiceProvider();
        var kafkaOptions = serviceProvider.GetRequiredService<IOptions<KafkaOptions>>();
        Assert.IsTrue(kafkaOptions is { Value: { Servers: "Kafka Server", ConnectionPoolSize: 10 } });
    }

    [TestMethod]
    public void TestSpecifyAssembliesShouldThrowException()
    {
        var builder = WebApplication.CreateBuilder();
        Assert.ThrowsException<MasaException>(() =>
        {
            return builder.AddMasaConfiguration(configurationBuilder =>
            {
                configurationBuilder
                    .AddJsonFile("rabbitMq.json", true, true)
                    .AddJsonFile("redis.json", true, true);
                configurationBuilder.UseMasaOptions(option => option.MappingConfigurationApi<MountSectionRedisOptions>("Test"));
            });
        }, $"Check if the mapping section is correct，section name is [{It.IsAny<string>()}]");
    }

    [TestMethod]
    public void TestNoParameterlessConstructorSpecifyAssembliesShouldThrowException()
    {
        var builder = WebApplication.CreateBuilder();
        Assert.ThrowsException<MasaException>(
            () => builder.AddMasaConfiguration(new[] { typeof(ConfigurationTest).Assembly, typeof(EsOptions).Assembly }),
            $"[{It.IsAny<string>()}] must have a parameterless constructor");
    }

    [TestMethod]
    public void TestRepeatMapptingShouldThrowException()
    {
        var builder = WebApplication.CreateBuilder();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
        {
            builder.AddMasaConfiguration(configurationBuilder =>
            {
                configurationBuilder.AddJsonFile("redis.json", true, true)
                    .AddJsonFile("rabbitMq.json", true, true);

                configurationBuilder.UseMasaOptions(option => option.MappingLocal<RedisOptions>().MappingLocal<RedisOptions>());
            });
        }, "The current section already has a configuration");
    }


    [TestMethod]
    public async Task TestConfigurationChangeShouldReturnNameEmpty()
    {
        var builder = WebApplication.CreateBuilder();
        var rootPath = builder.Environment.ContentRootPath;
        builder.AddMasaConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddJsonFile("customAppConfig.json", true, true)
                .AddJsonFile("rabbitMq.json", true, true);
            configurationBuilder.UseMasaOptions(option => option.MappingLocal<RedisOptions>());
        }, options => options.Assemblies = new[] { typeof(ConfigurationTest).Assembly });

        var serviceProvider = builder.Services.BuildServiceProvider();
        var redisOptions = serviceProvider.GetRequiredService<IOptions<RedisOptions>>();

        Assert.IsNotNull(redisOptions);
        Assert.IsTrue(redisOptions.Value.Ip == "localhost" && redisOptions.Value.Port == 6379);

        var newRedisOption = redisOptions.Value;
        newRedisOption.Ip = String.Empty;

        var oldContent = await File.ReadAllTextAsync(Path.Combine(rootPath, "customAppConfig.json"));
        await File.WriteAllTextAsync(Path.Combine(rootPath, "customAppConfig.json"),
            System.Text.Json.JsonSerializer.Serialize(new { RedisOptions = newRedisOption }));

        await Task.Delay(2000);

        var option = serviceProvider.GetRequiredService<IOptionsMonitor<RedisOptions>>();
        Assert.IsTrue(option.CurrentValue.Ip == "" && option.CurrentValue.Port == 6379);

        await File.WriteAllTextAsync(Path.Combine(rootPath, "customAppConfig.json"), oldContent);
    }

    [TestMethod]
    public void TestEnvironmentConfigurationMigrated()
    {
        var builder = WebApplication.CreateBuilder();
        Environment.SetEnvironmentVariable("project-name", "masa-unit-test");
        builder.AddMasaConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddJsonFile("customAppConfig.json", true, true)
                .AddJsonFile("rabbitMq.json", true, true);
            configurationBuilder.UseMasaOptions(option => option.MappingLocal<RedisOptions>());
        }, options =>
        {
            options.Assemblies = new[] { typeof(ConfigurationTest).Assembly };
            options.ExcludeConfigurationSourceTypes = new List<Type>();
            options.ExcludeConfigurationProviderTypes = new List<Type>();
        });

        Assert.IsTrue(builder.Configuration[$"{SectionTypes.Local}{ConfigurationPath.KeyDelimiter}project-name"] == "masa-unit-test");

    }

    [TestMethod]
    public void TestEnvironmentConfigurationNotMigrated()
    {
        var builder = WebApplication.CreateBuilder();
        Environment.SetEnvironmentVariable("project-name", "masa-unit-test");
        builder.AddMasaConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddJsonFile("customAppConfig.json", true, true)
                .AddJsonFile("rabbitMq.json", true, true);
            configurationBuilder.UseMasaOptions(option => option.MappingLocal<RedisOptions>());
        }, options =>
        {
            options.Assemblies = new[] { typeof(ConfigurationTest).Assembly };
        });

        Assert.IsTrue(builder.Configuration["project-name"] == "masa-unit-test");
    }

    [DataTestMethod]
    [DataRow("KafkaOptions:Servers", "Kafka Server")]
    [DataRow("kafkaoptions:Servers", "Kafka Server")]
    [DataRow("kafkaOptions:servers", "Kafka Server")]
    public void TestMasaConfigurationByKey(string key, string value)
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddMasaConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddJsonFile("customAppConfig.json", true, true)
                .AddJsonFile("rabbitMq.json", true, true);
            configurationBuilder.UseMasaOptions(option => option.MappingLocal<RedisOptions>());
        }, options =>
        {
            options.Assemblies = new[] { typeof(ConfigurationTest).Assembly };
        });
        var localConfiguration = builder.GetMasaConfiguration().Local;
        Assert.IsTrue(localConfiguration[key] == value);
    }

    [TestMethod]
    public void TestMasaConfigurationByName()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddMasaConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddJsonFile("customAppConfig.json", true, true)
                .AddJsonFile("rabbitMq.json", true, true);
            configurationBuilder.UseMasaOptions(option =>
            {
                option.MappingLocal<RedisOptions>();
                option.MappingLocal<RedisOptions>("RedisOptions2", "RedisOptions2");
            });
        }, options => options.Assemblies = new[] { typeof(ConfigurationTest).Assembly });
        var serviceProvider = builder.Services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptionsSnapshot<RedisOptions>>();
        Assert.IsNotNull(options);
        Assert.AreEqual("localhost", options.Value.Ip);
        Assert.AreEqual("", options.Value.Password);
        Assert.AreEqual(6379, options.Value.Port);

        var options2 = options.Get("RedisOptions2");
        Assert.AreEqual("127.0.0.1", options2.Ip);
        Assert.AreEqual("123456", options2.Password);
        Assert.AreEqual(6378, options2.Port);
    }

    [TestMethod]
    public void TestMasaConfigurationBuilderShouldReturnSourceCount3()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("rabbitMq.json", optional: false, reloadOnChange: true)
            .AddJsonFile("redis.json", optional: false, reloadOnChange: true);
        var masaConfigurationBuilder = new MasaConfigurationBuilder(new ServiceCollection(), configurationBuilder);
        Assert.IsTrue(masaConfigurationBuilder.Sources.Count == 2);

        var appsettingConfigurationBuilder =
            new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        Assert.IsTrue(appsettingConfigurationBuilder.Sources.Count == 1);
        masaConfigurationBuilder.Add(appsettingConfigurationBuilder.Sources.FirstOrDefault()!);
        Assert.IsTrue(masaConfigurationBuilder.Sources.Count == 3);

        Assert.IsTrue(masaConfigurationBuilder.Build()["KafkaOptions:Servers"] ==
            appsettingConfigurationBuilder.Build()["KafkaOptions:Servers"]);

        Assert.IsTrue(masaConfigurationBuilder.Properties.Count == configurationBuilder.Properties.Count);
    }

    [TestMethod]
    public void TestAddMasaConfiguration()
    {
        var services = new ServiceCollection();
        services.AddMasaConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddJsonFile("appsettings.json", false, true);
            configurationBuilder.AddJsonFile("rabbitMq.json", false, true);
        }, options =>
        {
            options.Assemblies = new[] { typeof(RabbitMqOptions).Assembly };
        });
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetService<IConfiguration>();
        Assert.IsNotNull(configuration);
        Assert.AreEqual("localhost", configuration["Local:RabbitMq:HostName"]);
        Assert.AreEqual("admin", configuration["Local:RabbitMq:UserName"]);
        Assert.AreEqual("admin", configuration["Local:RabbitMq:Password"]);
        Assert.AreEqual("/", configuration["Local:RabbitMq:VirtualHost"]);
        Assert.AreEqual("5672", configuration["Local:RabbitMq:Port"]);

        var options = serviceProvider.GetService<IOptions<RabbitMqOptions>>();
        Assert.IsNotNull(options);

        Assert.AreEqual("localhost", options.Value.HostName);
        Assert.AreEqual("admin", options.Value.UserName);
        Assert.AreEqual("admin", options.Value.Password);
        Assert.AreEqual("/", options.Value.VirtualHost);
        Assert.AreEqual("5672", options.Value.Port);
    }
}
#pragma warning restore CS0618
