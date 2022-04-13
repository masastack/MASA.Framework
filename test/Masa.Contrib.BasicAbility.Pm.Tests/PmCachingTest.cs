namespace Masa.Contrib.BasicAbility.Pm.Tests;

[TestClass]
public class PmCachingTest
{
    [TestMethod]
    public void TestAddPmClient()
    {
        var services = new ServiceCollection();

        services.AddPmClient(option =>
        {
            option.UseHttpClient(builder =>
            {
                builder.Name = "masa.contrib.basicability.pm";
                builder.Configure = opt => opt.BaseAddress = new Uri("https://github.com");
            });
        });

        var pmClient = services.BuildServiceProvider().GetRequiredService<IPmClient>();
        Assert.IsNotNull(pmClient);
    }

    [TestMethod]
    public void TestAddMultiplePmClient()
    {
        var services = new ServiceCollection();

        services.AddPmClient(option =>
        {
            option.UseHttpClient(builder =>
            {
                builder.Name = "masa.contrib.basicability.pm";
                builder.Configure = opt => opt.BaseAddress = new Uri("https://github.com");
            });
        });

        services.AddPmClient(option =>
        {
            option.UseHttpClient(builder =>
            {
                builder.Name = "masa.contrib.basicability.pm";
                builder.Configure = opt => opt.BaseAddress = new Uri("https://github.com");
            });
        });

        var pmClient = services.BuildServiceProvider().GetRequiredService<IPmClient>();
        Assert.IsNotNull(pmClient);
    }


    [TestMethod]
    public async Task TestGetProjectAppsListAsync()
    {
        var list = new List<ProjectModel>()
        {
            new ProjectModel(1, "", "", 1, Guid.NewGuid())
        };
        string env = "development";
        var requestUri = $"api/v1/projectwithapps/{env}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<ProjectModel>>(requestUri, default).Result).Returns(list).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var data = await pmCaching.ProjectService.GetProjectListAsync(env);
        callerProvider.Verify(provider => provider.GetAsync<List<ProjectModel>>(requestUri, default), Times.Once);
        Assert.IsTrue(data.Count == 1);
    }

    [TestMethod]
    public async Task TestGetProjectAppsList2Async()
    {
        List<ProjectModel> list = null!;
        string env = "development";
        var requestUri = $"api/v1/projectwithapps/{env}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => (provider.GetAsync<List<ProjectModel>>(It.IsAny<string>(), default))).ReturnsAsync(list)
            .Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var data = await pmCaching.ProjectService.GetProjectListAsync(env);
        callerProvider.Verify(provider => provider.GetAsync<List<ProjectModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(data.Count == 0);
    }

    [TestMethod]
    public void TestGetEnvironmentService()
    {
        var callerProvider = new Mock<ICallerProvider>();
        var pmCaching = new PmClient(callerProvider.Object);

        var env = pmCaching.EnvironmentService;

        Assert.IsNotNull(env);
    }

    [TestMethod]
    public void TestGetClusterService()
    {
        var callerProvider = new Mock<ICallerProvider>();
        var pmCaching = new PmClient(callerProvider.Object);

        var env = pmCaching.ClusterService;

        Assert.IsNotNull(env);
    }

    [TestMethod]
    public void TestGetAppService()
    {
        var callerProvider = new Mock<ICallerProvider>();
        var pmCaching = new PmClient(callerProvider.Object);

        var env = pmCaching.AppService;

        Assert.IsNotNull(env);
    }

    [TestMethod]
    public void TestNullCallerOptionsReturnThrowArgumentNullException()
    {
        var services = new ServiceCollection();
        Action<CallerOptions> callerOptions = null!;
        Assert.ThrowsException<ArgumentNullException>(() => services.AddPmClient(callerOptions));
    }
}
