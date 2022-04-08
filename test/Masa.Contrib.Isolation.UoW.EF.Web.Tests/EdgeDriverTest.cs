namespace Masa.Contrib.Isolation.UoW.EF.Web.Tests;

[TestClass]
public class EdgeDriverTest
{
    private IServiceCollection _services;

    [TestInitialize]
    public void Initialize()
    {
        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        _services = new ServiceCollection();
        _services.AddSingleton<IConfiguration>(configurationRoot);
        _services.AddEventBus(eventBusBuilder => eventBusBuilder.UseIsolationUoW<CustomDbContext, int>(
            isolationBuilder => isolationBuilder.UseMultiTenant("tenant").UseMultiEnvironment("env"), dbOptions => dbOptions.UseSqlite()));
        System.Environment.SetEnvironmentVariable("env", "pro");
    }

    [TestMethod]
    public async Task TestTenantAsync()
    {
        var serviceProvider = _services.BuildServiceProvider();

        #region Manually assign values to tenants and environments, and in real scenarios, automatically parse and assign values based on the current HttpContext

        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext();
        httpContextAccessor.HttpContext.Items = new Dictionary<object, object?>
        {
            { "tenant", "2" }
        };

        #endregion

        var registerUserEvent = new RegisterUserEvent("jim", "123456");
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        await eventBus.PublishAsync(registerUserEvent);
    }

    [TestMethod]
    public async Task TestTenant2Async()
    {
        var serviceProvider = _services.BuildServiceProvider();

        #region Manually assign values to tenants and environments, and in real scenarios, automatically parse and assign values based on the current HttpContext

        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext();
        httpContextAccessor.HttpContext.Items = new Dictionary<object, object?>
        {
            { "tenant", "1" }
        };

        #endregion

        var addRoleEvent = new AddRoleEvent("Admin", 1);
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        await eventBus.PublishAsync(addRoleEvent);
    }
}
