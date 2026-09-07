// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Dapr.Tests;

[TestClass]
public class DaprConfigurationApiClientTest
{
    private const string DefaultDaprHttpEndpoint = "http://127.0.0.1:3500";
    private const string DefaultRedisConnection = "127.0.0.1:6379";
    private const string DefaultDaprStoreName = "configuration";

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly List<string> _seededKeys = new();

    private ConnectionMultiplexer? _redisConnection;
    private DaprClient? _daprClient;

    [TestCleanup]
    public async Task CleanupAsync()
    {
        if (_redisConnection != null && _seededKeys.Count > 0)
        {
            var db = _redisConnection.GetDatabase();
            foreach (var key in _seededKeys)
            {
                await db.KeyDeleteAsync(key).ConfigureAwait(false);
            }
        }

        _daprClient?.Dispose();
        if (_redisConnection != null)
        {
            await _redisConnection.CloseAsync(true).ConfigureAwait(false);
            _redisConnection.Dispose();
        }
    }

    [TestMethod]
    public async Task GetRawAsync_ShouldReadPublishReleaseModelFromDaprConfigurationStore()
    {
        var context = await CreateContextAsync().ConfigureAwait(false);
        await SeedJsonConfigurationAsync(context).ConfigureAwait(false);

        var result = await context.Client.GetRawAsync(context.Environment, context.Cluster, context.AppId, context.ConfigObject)
            .ConfigureAwait(false);

        Assert.AreEqual(ConfigurationTypes.Json, result.ConfigurationType);
        using var doc = JsonDocument.Parse(result.Raw);
        Assert.AreEqual("Masa", doc.RootElement.GetProperty("Name").GetString());
    }

    [TestMethod]
    public async Task GetAsync_ShouldReturnStrongTypedObject()
    {
        var context = await CreateContextAsync().ConfigureAwait(false);
        await SeedJsonConfigurationAsync(context).ConfigureAwait(false);

        var result = await context.Client.GetAsync<Brand>(context.Environment, context.Cluster, context.AppId, context.ConfigObject)
            .ConfigureAwait(false);

        Assert.IsNotNull(result);
        Assert.AreEqual("Masa", result.Name);
        Assert.AreEqual("1", result.Id);
    }

    [TestMethod]
    public async Task GetDynamicAsync_ShouldReturnDynamicObject()
    {
        var context = await CreateContextAsync().ConfigureAwait(false);
        await SeedJsonConfigurationAsync(context).ConfigureAwait(false);

        dynamic result = await context.Client
            .GetDynamicAsync(context.Environment, context.Cluster, context.AppId, context.ConfigObject)
            .ConfigureAwait(false);

        Assert.IsNotNull(result);
        Assert.AreEqual("Masa", (string)result.Name);
        Assert.AreEqual("1", (string)result.Id);
        Assert.AreEqual(2, ((ExpandoObject)result).Count());
    }

    private async Task<TestContextModel> CreateContextAsync()
    {
        var daprHttpEndpoint = Environment.GetEnvironmentVariable("DAPR_HTTP_ENDPOINT") ?? DefaultDaprHttpEndpoint;
        var redisConnectionString = Environment.GetEnvironmentVariable("DAPR_TEST_REDIS_CONNECTION") ?? DefaultRedisConnection;
        var storeName = Environment.GetEnvironmentVariable("DAPR_TEST_CONFIGURATION_STORE") ?? DefaultDaprStoreName;

        _redisConnection = await ConnectRedisOrInconclusiveAsync(redisConnectionString).ConfigureAwait(false);
        _daprClient = CreateDaprClient(daprHttpEndpoint);
        await EnsureDaprConfigurationStoreReachableAsync(_daprClient, storeName).ConfigureAwait(false);

        var services = new ServiceCollection();
        services.AddSingleton(_daprClient);
        var serviceProvider = services.BuildServiceProvider();

        var environment = "Development";
        var cluster = "Default";
        var appId = "DaprUnitTestApp";
        var configObject = $"Brand-{Guid.NewGuid():N}";
        var expectedKey = $"{Constants.DEFAULT_PREFIX}{environment}-{cluster}-{appId}-{configObject}".ToLowerInvariant();

        var option = new DccDaprOptions
        {
            StoreName = storeName,
            ConfigObjectSecret = "Secret",
            Environment = environment,
            Cluster = cluster,
            AppId = appId,
            ConfigObjects = new List<string> { configObject },
            ExpandSections = new List<DccSectionOptions>(),
            ManageServiceAddress = "http://127.0.0.1:5179",
            Secret = "Secret"
        };

        var client = new DaprConfigurationApiClient(serviceProvider, _jsonSerializerOptions, option, option.ExpandSections);
        return new TestContextModel(client, environment, cluster, appId, configObject, expectedKey);
    }

    private async Task SeedJsonConfigurationAsync(TestContextModel context)
    {
        var payload = new Brand("Masa", "1");
        var releaseModel = new PublishReleaseModel
        {
            ConfigFormat = ConfigFormats.JSON,
            Content = JsonSerializer.Serialize(payload, _jsonSerializerOptions)
        };
        var publishContent = JsonSerializer.Serialize(releaseModel, _jsonSerializerOptions);
        var db = _redisConnection!.GetDatabase();
        await db.StringSetAsync(context.ExpectedKey, publishContent).ConfigureAwait(false);
        _seededKeys.Add(context.ExpectedKey);
    }

    private static DaprClient CreateDaprClient(string daprHttpEndpoint)
    {
        return new DaprClientBuilder()
            .UseHttpEndpoint(daprHttpEndpoint)
            .Build();
    }

    private static async Task<ConnectionMultiplexer> ConnectRedisOrInconclusiveAsync(string redisConnectionString)
    {
        try
        {
            var options = StackExchange.Redis.ConfigurationOptions.Parse(redisConnectionString);
            options.AbortOnConnectFail = false;
            options.ConnectTimeout = 2000;
            options.SyncTimeout = 2000;
            return await ConnectionMultiplexer.ConnectAsync(options).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Assert.Inconclusive($"Redis is unavailable. Start redis and retry. connection={redisConnectionString}, error={ex.Message}");
            throw;
        }
    }

    private static readonly string[] HealthCheckKeys = { "__masa_health_check__" };

    private static async Task EnsureDaprConfigurationStoreReachableAsync(DaprClient daprClient, string storeName)
    {
        try
        {
#pragma warning disable CS0618
            await daprClient.GetConfiguration(storeName, HealthCheckKeys).ConfigureAwait(false);
#pragma warning restore CS0618
        }
        catch (Exception ex)
        {
            Assert.Inconclusive(
                $"Dapr configuration store is unavailable. Start daprd and load configuration component. store={storeName}, error={ex.Message}");
            throw;
        }
    }

    private sealed record TestContextModel(
        DaprConfigurationApiClient Client,
        string Environment,
        string Cluster,
        string AppId,
        string ConfigObject,
        string ExpectedKey);

    private sealed record Brand(string Name, string Id);
}
