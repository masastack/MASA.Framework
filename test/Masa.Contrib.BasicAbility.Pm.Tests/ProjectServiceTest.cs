// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Pm.Tests;

[TestClass]
public class ProjectServiceTest
{
    [TestMethod]
    public async Task TestGetProjectAppsAsync()
    {
        var data = new List<ProjectAppsModel>()
        {
            new ProjectAppsModel(1, "", "", "", Guid.NewGuid())
        };
        string env = "development";
        var requestUri = $"api/v1/projectwithapps/{env}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<ProjectAppsModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.ProjectService.GetProjectAppsAsync(env);
        callerProvider.Verify(provider => provider.GetAsync<List<ProjectAppsModel>>(requestUri, default), Times.Once);
        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    public async Task TestGetProjectApps2Async()
    {
        List<ProjectAppsModel>? data = null;
        string env = "development";
        var requestUri = $"api/v1/projectwithapps/{env}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => (provider.GetAsync<List<ProjectAppsModel>>(It.IsAny<string>(), default))).ReturnsAsync(data)
            .Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.ProjectService.GetProjectAppsAsync(env);
        callerProvider.Verify(provider => provider.GetAsync<List<ProjectAppsModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 0);
    }

    [TestMethod]
    [DataRow(1)]
    public async Task TestGetAsync(int Id)
    {
        var data = new ProjectDetailModel();

        var requestUri = $"api/v1/project/{Id}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<ProjectDetailModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.ProjectService.GetAsync(Id);
        callerProvider.Verify(provider => provider.GetAsync<ProjectDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow(1)]
    public async Task TestGet1Async(int id)
    {
        ProjectDetailModel? data = null;

        var requestUri = $"api/v1/project/{id}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<ProjectDetailModel>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.ProjectService.GetAsync(id);
        callerProvider.Verify(provider => provider.GetAsync<ProjectDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow("identity")]
    public async Task TestGetByIdentityAsync(string identity)
    {
        var data = new ProjectDetailModel();

        var requestUri = $"open-api/project/{identity}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<ProjectDetailModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.ProjectService.GetByIdentityAsync(identity);
        callerProvider.Verify(provider => provider.GetAsync<ProjectDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow("identity")]
    public async Task TestGetByIdentity1Async(string identity)
    {
        ProjectDetailModel? data = null;

        var requestUri = $"open-api/project/{identity}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<ProjectDetailModel>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.ProjectService.GetByIdentityAsync(identity);
        callerProvider.Verify(provider => provider.GetAsync<ProjectDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow(1)]
    public async Task TestGetListByEnvironmentClusterIdAsync(int envClusterId)
    {
        var data = new List<ProjectModel>()
        {
            new ProjectModel { Id = 1 }
        };
        var requestUri = $"api/v1/{envClusterId}/project";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<ProjectModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.ProjectService.GetListByEnvironmentClusterIdAsync(envClusterId);
        callerProvider.Verify(provider => provider.GetAsync<List<ProjectModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    [DataRow(1)]
    public async Task TestGetListByEnvironmentClusterId1Async(int envClusterId)
    {
        List<ProjectModel>? data = null;
        var requestUri = $"api/v1/{envClusterId}/project";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<ProjectModel>>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.ProjectService.GetListByEnvironmentClusterIdAsync(envClusterId);
        callerProvider.Verify(provider => provider.GetAsync<List<ProjectModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 0);
    }

    [TestMethod]
    public async Task TestGetListByTeamIdsAsync()
    {
        var data = new List<ProjectModel>()
        {
            new ProjectModel { Id = 1 }
        };
        var teamIds = new List<Guid> { Guid.NewGuid() };
        var requestUri = $"api/v1/project/teamProjects";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.PostAsync<List<ProjectModel>>(requestUri, teamIds, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.ProjectService.GetListByTeamIdsAsync(teamIds);
        callerProvider.Verify(provider => provider.PostAsync<List<ProjectModel>>(requestUri, teamIds, default), Times.Once);

        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    public async Task TestGetListByTeamIds1Async()
    {
        List<ProjectModel>? data = null;
        var teamIds = new List<Guid> { Guid.NewGuid() };
        var requestUri = $"api/v1/project/teamProjects";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.PostAsync<List<ProjectModel>>(It.IsAny<string>(), It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.ProjectService.GetListByTeamIdsAsync(teamIds);
        callerProvider.Verify(provider => provider.PostAsync<List<ProjectModel>>(requestUri, teamIds, default), Times.Once);

        Assert.IsTrue(result.Count == 0);
    }

    [TestMethod]
    public async Task TestGetProjectTypesAsync()
    {
        var data = new List<ProjectTypeModel>()
        {
            new ProjectTypeModel { Id = 1 }
        };
        var requestUri = $"api/v1/project/projectType";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<ProjectTypeModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.ProjectService.GetProjectTypesAsync();
        callerProvider.Verify(provider => provider.GetAsync<List<ProjectTypeModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    public async Task TestGetProjectTypes1Async()
    {
        List<ProjectTypeModel>? data = null;
        var requestUri = $"api/v1/project/projectType";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<ProjectTypeModel>>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.ProjectService.GetProjectTypesAsync();
        callerProvider.Verify(provider => provider.GetAsync<List<ProjectTypeModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 0);
    }
}
