using MASA.Contrib.Configuration.ErrorSectionAutoMapTests;
using MASA.Contrib.Configuration.MountErrorSectionAutoMapTests;

namespace MASA.Contrib.Configuration.Tests;

[TestClass]
public class ConfigurationTest
{
    private IConfigurationBuilder _configurationBuilder;

    [TestInitialize]
    public void Initialize()
    {
        _configurationBuilder = new ConfigurationBuilder();
    }

    [TestMethod]
    public void TestAddSection()
    {
        var masaConfigurationBuilder = new MasaConfigurationBuilder(_configurationBuilder);
        Assert.ThrowsException<ArgumentNullException>(() => masaConfigurationBuilder.AddSection(null));

        Assert.ThrowsException<ArgumentException>(() => masaConfigurationBuilder.AddSection(new ConfigurationBuilder()));

        masaConfigurationBuilder.AddSection(
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true), "appsettings"
        );

        Assert.IsTrue(masaConfigurationBuilder.GetSectionRelations().Count == 1);

        masaConfigurationBuilder.AddSection(
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("redis.json", true, true)
        );
        Assert.IsTrue(masaConfigurationBuilder.GetSectionRelations().Count == 2);

        Assert.ThrowsException<ArgumentException>(() =>
        {
            masaConfigurationBuilder.AddSection(
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("rabbitMq.json", true, true)
            );
        });
    }

    [TestMethod]
    public void TestAddCustomSection()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddMasaConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddSection(new ConfigurationBuilder()
               .SetBasePath(builder.Environment.ContentRootPath)
               .AddJsonFile("appsettings.json", true, true), "Appsettings");

            configurationBuilder.AddSection(new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("redis.json", true, true), "RedisOptions");

            configurationBuilder.AddSection(
               new ConfigurationBuilder()
                   .SetBasePath(builder.Environment.ContentRootPath)
                   .AddJsonFile("rabbitMq.json", true, true), "RabbitMqOptions"
           );

            configurationBuilder.UseMasaOptions(option =>
            {
                option.Mapping<RedisOptions>(SectionTypes.Local, "");
            });
        });
        var serviceProvider = builder.Services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var redisOption = serviceProvider.GetRequiredService<IOptions<RedisOptions>>();

        Assert.IsNotNull(configuration);
        Assert.IsNotNull(redisOption);
        Assert.IsTrue(redisOption.Value.Ip == "localhost");
    }

    [TestMethod]
    public void TestAddMasaConfiguration()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddMasaConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddSection(new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true), "Appsettings");

            configurationBuilder.AddSection(new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("redis.json", true, true)
            );
            configurationBuilder.AddSection(new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("rabbitMq.json", true, true), "RabbitMqOptions"
            );
            configurationBuilder.UseMasaOptions(option =>
                option.Mapping<RedisOptions>(SectionTypes.Local, "", "")
            );
        });
        var serviceProvider = builder.Services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var redisOption = serviceProvider.GetRequiredService<IOptions<RedisOptions>>();
        Assert.IsTrue(configuration["Local:Ip"] == "localhost");
        Assert.IsTrue(redisOption.Value.Ip == "localhost");

        var rabbitMqOption = serviceProvider.GetRequiredService<IOptions<RabbitMqOptions>>();
        Assert.IsTrue(configuration["Local:RabbitMqOptions:UserName"] == "admin");
        Assert.IsTrue(rabbitMqOption.Value.UserName == "admin" && rabbitMqOption.Value.Password == "admin");
    }

    [TestMethod]
    public void TestAddMultiMasaConfiguration()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddMasaConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddSection(new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true), "Appsettings");

            configurationBuilder.AddSection(new ConfigurationBuilder()
                    .SetBasePath(builder.Environment.ContentRootPath)
                    .AddJsonFile("redis.json", true, true)
            );
            configurationBuilder.AddSection(new ConfigurationBuilder()
                    .SetBasePath(builder.Environment.ContentRootPath)
                    .AddJsonFile("rabbitMq.json", true, true), "RabbitMqOptions"
            );
            configurationBuilder.UseMasaOptions(option =>
                option.Mapping<RedisOptions>(SectionTypes.Local, "", "")
            );
        }).AddMasaConfiguration();
        var serviceProvider = builder.Services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var redisOption = serviceProvider.GetRequiredService<IOptions<RedisOptions>>();
        Assert.IsTrue(configuration["Local:Ip"] == "localhost");
        Assert.IsTrue(redisOption.Value.Ip == "localhost");

        var rabbitMqOption = serviceProvider.GetRequiredService<IOptions<RabbitMqOptions>>();
        Assert.IsTrue(configuration["Local:RabbitMqOptions:UserName"] == "admin");
        Assert.IsTrue(rabbitMqOption.Value.UserName == "admin" && rabbitMqOption.Value.Password == "admin");
    }

    [TestMethod]
    public void TestAutoMapSectionError()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Host.ConfigureAppConfiguration((context, config) => { config.Sources.Clear(); });
        var chainedConfiguration = new ConfigurationBuilder()
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true);
        builder.Configuration.AddConfiguration(chainedConfiguration.Build());

        Assert.ThrowsException<ArgumentException>(() =>
            builder.AddMasaConfiguration(configurationBuilder =>
            {
                configurationBuilder.AddSection(
                new ConfigurationBuilder()
                    .SetBasePath(builder.Environment.ContentRootPath)
                    .AddJsonFile("appsettings.json", true, true), "Appsettings"
                    );
            }, typeof(ConfigurationTest).Assembly, typeof(KafkaOptions).Assembly));
    }

    [TestMethod]
    public void TestAutoMapAndErrorSection()
    {
        var builder = WebApplication.CreateBuilder();
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            return builder.AddMasaConfiguration(configurationBuilder =>
            {
                configurationBuilder.AddSection(new ConfigurationBuilder()
                    .SetBasePath(builder.Environment.ContentRootPath)
                    .AddJsonFile("appsettings.json", true, true), "Appsettings"
                   );

                configurationBuilder.AddSection(new ConfigurationBuilder()
                    .SetBasePath(builder.Environment.ContentRootPath)
                    .AddJsonFile("redis.json", true, true)
                ); //Mount to the Local section
            }, typeof(ConfigurationTest).Assembly, typeof(MountSectionRedisOptions).Assembly);
        });
    }

    [TestMethod]
    public void TestRepeatMappting()
    {
        var builder = WebApplication.CreateBuilder();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
        {
            builder.AddMasaConfiguration(configurationBuilder =>
            {
                configurationBuilder.AddSection(new ConfigurationBuilder()
                    .SetBasePath(builder.Environment.ContentRootPath)
                    .AddJsonFile("redis.json", true, true)
                );
                configurationBuilder.AddSection(new ConfigurationBuilder()
                    .SetBasePath(builder.Environment.ContentRootPath)
                    .AddJsonFile("rabbitMq.json", true, true), "RabbitMqOptions"
                );
                configurationBuilder.UseMasaOptions(option =>
                {
                    option.Mapping<RedisOptions>(SectionTypes.Local, "", "");
                    option.Mapping<RedisOptions>(SectionTypes.Local, "", "");
                });
            });
        });
    }

    [TestMethod]
    public void TestCreateMasaConfiguration()
    {
        var services = new ServiceCollection();
        services.CreateMasaConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddSection(
                new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("redis.json", true, true)
            );
            configurationBuilder.AddSection(
                new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("rabbitMq.json", true, true), "RabbitMqOptions"
            );
            configurationBuilder.UseMasaOptions(option =>
                option.Mapping<RedisOptions>(SectionTypes.Local, "", "")
            );
        }, new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true), "Appsettings");
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        var redisOption = serviceProvider.GetRequiredService<IOptions<RedisOptions>>();
        Assert.IsTrue(redisOption.Value.Ip == "localhost");
    }

    [TestMethod]
    public void TestNullSection()
    {
        var services = new ServiceCollection();
        var ex = Assert.ThrowsException<Exception>(() => services.CreateMasaConfiguration(null));
        Assert.IsTrue(ex.Message == "Please add the section to be loaded");
    }

    [TestMethod]
    public void TestConfigurationChange()
    {
        var builder = WebApplication.CreateBuilder();

        var rootPath = builder.Environment.ContentRootPath;
        builder.AddMasaConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddSection(new ConfigurationBuilder()
                .SetBasePath(rootPath)
                .AddJsonFile("appsettings.json", true, true), "Appsettings");

            configurationBuilder.AddSection(new ConfigurationBuilder()
                .SetBasePath(rootPath)
                .AddJsonFile("redis.json", true, true), "RedisOptions");

            configurationBuilder.AddSection(new ConfigurationBuilder()
                .SetBasePath(rootPath)
                .AddJsonFile("rabbitMq.json", true, true), "RabbitMqOptions"
           );

            configurationBuilder.UseMasaOptions(option =>
            {
                option.Mapping<RedisOptions>(SectionTypes.Local, "");
            });
        }, typeof(ConfigurationTest).Assembly);
        var serviceProvider = builder.Services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var systemOption = serviceProvider.GetRequiredService<IOptions<SystemOptions>>();

        Assert.IsNotNull(configuration);
        Assert.IsNotNull(systemOption);
        Assert.IsTrue(systemOption.Value.Name == "MASA TEST");

        var newRedisOption = systemOption.Value;
        newRedisOption.Name = null;

        File.WriteAllText(Path.Combine(rootPath, "appsettings.json"), System.Text.Json.JsonSerializer.Serialize(new { SystemOptions = newRedisOption }));

        Thread.Sleep(2000);
        var option = serviceProvider.GetRequiredService<IOptionsMonitor<SystemOptions>>();
        Assert.IsTrue(option.CurrentValue.Name == "");
    }
}
