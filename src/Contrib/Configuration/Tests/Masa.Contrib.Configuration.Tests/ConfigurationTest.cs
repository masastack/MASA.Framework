// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.Tests;

[TestClass]
public class ConfigurationTest
{
    [DataRow(true, true, true, true, true)]
    [DataRow(true, true, false, true, false)]
    [DataRow(true, false, true, true, false)]
    [DataRow(true, false, false, true, false)]
    [DataRow(false, true, true, false, true)]
    [DataRow(false, true, false, false, false)]
    [DataRow(false, false, true, false, false)]
    [DataRow(false, false, false, false, false)]
    [DataTestMethod]
    public void TestAddMasaConfiguration(
        bool isAddRabbitMq,
        bool isAddRedis,
        bool isMappingRedis,
        bool expectedRabbitMq,
        bool expectedRedis)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMasaConfiguration(optionsBuilder =>
        {
            if (isMappingRedis)
            {
                optionsBuilder.UseMasaOptions(options =>
                {
                    options.MappingLocal<RedisOptions>();
                });
            }

            optionsBuilder.ConfigurationBuilderAction = (configurationBuilder, _) =>
            {
                if (isAddRabbitMq) configurationBuilder.AddJsonFile("rabbitMq.json", optional: false, reloadOnChange: true);
                if (isAddRedis) configurationBuilder.AddJsonFile("redis.json", optional: false, reloadOnChange: true);
            };
        });

        var serviceProvider = builder.Services.BuildServiceProvider();

        var rabbitMqOptions = serviceProvider.GetRequiredService<IOptions<RabbitMqOptions>>();
        Assert.AreEqual(expectedRabbitMq,
            rabbitMqOptions is { Value: { HostName: "localhost", UserName: "admin", Password: "admin", VirtualHost: "/", Port: "5672" } });

        var systemOptions = serviceProvider.GetRequiredService<IOptions<SystemOptions>>();
        Assert.IsTrue(systemOptions is { Value.Name: "Masa TEST" });

        var redisOptions = serviceProvider.GetRequiredService<IOptions<RedisOptions>>();
        Assert.AreEqual(expectedRedis, redisOptions is { Value: { Ip: "localhost", Password: "", Port: 6379 } });

        Assert.AreEqual("Kafka Server", builder.Configuration["KafkaOptions:Servers"]);
        Assert.AreEqual("10", builder.Configuration["KafkaOptions:ConnectionPoolSize"]);
        Assert.AreEqual("Masa TEST", builder.Configuration["SystemOptions:Name"]);
        Assert.AreEqual("http://localhost:9200", builder.Configuration["EsOptions:Nodes:0"]);
        Assert.AreEqual("Test", builder.Configuration["Env"]);
        Assert.AreEqual("masa", builder.Configuration["masa-dev"]);
    }

    [TestMethod]
    public void TestAddMasaConfigurationByDisableAuto()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMasaConfiguration(optionsBuilder =>
        {
            optionsBuilder.UseMasaOptions(options =>
            {
                options.MappingLocal<RedisOptions>();
            });

            optionsBuilder.ConfigurationBuilderAction = (configurationBuilder, _) =>
            {
                configurationBuilder.AddJsonFile("rabbitMq.json", optional: false, reloadOnChange: true);
                configurationBuilder.AddJsonFile("redis.json", optional: false, reloadOnChange: true);
            };
            optionsBuilder.EnableAutoMapOptions = false;
        });

        var serviceProvider = builder.Services.BuildServiceProvider();

        var rabbitMqOptions = serviceProvider.GetRequiredService<IOptions<RabbitMqOptions>>();
        Assert.AreEqual(true,
            rabbitMqOptions is { Value: { HostName: null, UserName: null, Password: null, VirtualHost: null, Port: null } });

        var systemOptions = serviceProvider.GetRequiredService<IOptions<SystemOptions>>();
        Assert.IsTrue(systemOptions is { Value.Name: null });

        var redisOptions = serviceProvider.GetRequiredService<IOptions<RedisOptions>>();
        Assert.AreEqual(true, redisOptions is { Value: { Ip: "localhost", Password: "", Port: 6379 } });

        Assert.AreEqual("Kafka Server", builder.Configuration["KafkaOptions:Servers"]);
        Assert.AreEqual("10", builder.Configuration["KafkaOptions:ConnectionPoolSize"]);
        Assert.AreEqual("Masa TEST", builder.Configuration["SystemOptions:Name"]);
        Assert.AreEqual("http://localhost:9200", builder.Configuration["EsOptions:Nodes:0"]);
        Assert.AreEqual("Test", builder.Configuration["Env"]);
        Assert.AreEqual("masa", builder.Configuration["masa-dev"]);
    }

    [TestMethod]
    public void TestSpecifyAssembliesShouldKafKaOptionsExist()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMasaConfiguration(typeof(KafkaOptions).Assembly);

        var serviceProvider = builder.Services.BuildServiceProvider();
        var kafkaOptions = serviceProvider.GetRequiredService<IOptions<KafkaOptions>>();
        Assert.IsTrue(kafkaOptions is { Value: { Servers: "Kafka Server", ConnectionPoolSize: 10 } });
    }

    [TestMethod]
    public async Task TestConfigurationChangeShouldReturnNameEmpty()
    {
        var builder = WebApplication.CreateBuilder();
        var rootPath = builder.Environment.ContentRootPath;
        builder.Services.AddMasaConfiguration(optionsBuilder =>
        {
            optionsBuilder.ConfigurationBuilderAction = (configurationBuilder, _) =>
            {
                configurationBuilder.AddJsonFile("customAppConfig.json", true, true)
                    .AddJsonFile("rabbitMq.json", true, true);
            };
            optionsBuilder.UseMasaOptions(option => option.MappingLocal<RedisOptions>());
            optionsBuilder.Assemblies = new[]
            {
                typeof(ConfigurationTest).Assembly
            };
        });

        var serviceProvider = builder.Services.BuildServiceProvider();
        var redisOptions = serviceProvider.GetRequiredService<IOptions<RedisOptions>>();

        Assert.IsNotNull(redisOptions);
        Assert.IsTrue(redisOptions.Value.Ip == "localhost" && redisOptions.Value.Port == 6379);

        var newRedisOption = redisOptions.Value;
        newRedisOption.Ip = string.Empty;
        newRedisOption.Port = 6378;

        var oldContent = await File.ReadAllTextAsync(Path.Combine(rootPath, "customAppConfig.json"));
        await File.WriteAllTextAsync(Path.Combine(rootPath, "customAppConfig.json"),
            System.Text.Json.JsonSerializer.Serialize(new
            {
                RedisOptions = newRedisOption
            }));

        await Task.Delay(2000);

        var option = serviceProvider.GetRequiredService<IOptionsMonitor<RedisOptions>>();
        Assert.IsTrue(option.CurrentValue.Ip == string.Empty && option.CurrentValue.Port == 6378);

        await File.WriteAllTextAsync(Path.Combine(rootPath, "customAppConfig.json"), oldContent);
    }

    [TestMethod]
    public void TestMasaConfigurationByName()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMasaConfiguration(optionsBuilder =>
        {
            optionsBuilder.ConfigurationBuilderAction = (configurationBuilder, _) =>
                configurationBuilder
                    .AddJsonFile("customAppConfig.json", true, true)
                    .AddJsonFile("rabbitMq.json", true, true);
            optionsBuilder.UseMasaOptions(option =>
            {
                option.MappingLocal<RedisOptions>();
                option.MappingLocal<RedisOptions>("RedisOptions2", "RedisOptions2");
            });
            optionsBuilder.Assemblies = new[]
            {
                typeof(ConfigurationTest).Assembly
            };
        });
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
    public void TestMasaConfigurationByMultiEnvironment()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMasaConfiguration(optionsBuilder =>
        {
            optionsBuilder.EnableAutoMapOptions = false;
            optionsBuilder.UseMasaOptions(options =>
            {
                options.MappingLocal<RedisOptions>("RedisOptionsNew");
            });
            optionsBuilder.ConfigurationBuilderAction = (configurationBuilder, serviceProvider) =>
            {
                configurationBuilder.AddJsonFile("redis.json", true, false);
                var env = serviceProvider.GetRequiredService<IMultiEnvironmentContext>().CurrentEnvironment;
                configurationBuilder.AddJsonFile($"redis.{env}.json", true, false);
            };
        });
        builder.Services.AddIsolation(isolationOptions => isolationOptions.UseMultiEnvironment());
        var rootServiceProvider = builder.Services.BuildServiceProvider();
        using (var scope = rootServiceProvider.CreateScope())
        {
            var multiEnvironmentSetter = scope.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>();
            multiEnvironmentSetter.SetEnvironment("test");

            var options = scope.ServiceProvider.GetRequiredService<IOptions<RedisOptions>>();
            Assert.AreEqual("localhost-test", options.Value.Ip);
            Assert.AreEqual("test", options.Value.Password);
            Assert.AreEqual(6376, options.Value.Port);

            var optionsSnapshot = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<RedisOptions>>();
            Assert.AreEqual("localhost-test", optionsSnapshot.Value.Ip);
            Assert.AreEqual("test", optionsSnapshot.Value.Password);
            Assert.AreEqual(6376, optionsSnapshot.Value.Port);

            var serviceProvider = scope.ServiceProvider;
            Assert.ThrowsException<NotSupportedException>(() => serviceProvider.GetRequiredService<IOptionsMonitor<RedisOptions>>().CurrentValue);

            var isolationOptions = serviceProvider.GetRequiredService<IOptionsMonitor<IsolationOptions>>();
            Assert.IsNotNull(isolationOptions);
            Assert.AreEqual( IsolationConstant.DEFAULT_MULTI_ENVIRONMENT_NAME, isolationOptions.CurrentValue.MultiEnvironmentName);
        }

        using (var scope = rootServiceProvider.CreateScope())
        {
            var multiEnvironmentSetter = scope.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>();
            multiEnvironmentSetter.SetEnvironment("dev");
            var options = scope.ServiceProvider.GetRequiredService<IOptions<RedisOptions>>();
            Assert.AreEqual("localhost-dev", options.Value.Ip);
            Assert.AreEqual("dev", options.Value.Password);
            Assert.AreEqual(6377, options.Value.Port);
        }
        using (var scope = rootServiceProvider.CreateScope())
        {
            var multiEnvironmentSetter = scope.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>();
            multiEnvironmentSetter.SetEnvironment("production");
            var options = scope.ServiceProvider.GetRequiredService<IOptions<RedisOptions>>();
            Assert.AreEqual("localhost", options.Value.Ip);
            Assert.AreEqual("", options.Value.Password);
            Assert.AreEqual(6378, options.Value.Port);
        }
    }
}
