// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.StackSdks.Auth.Service;

namespace Masa.Contrib.StackSdks.Auth.Tests;

[TestClass]
public class ProjectServiceTest
{
    [TestMethod]
    public async Task TestGetGlobalNavigationsAsync()
    {
        var data = new List<ProjectModel>()
        {
            new ProjectModel()
        };
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var requestUri = $"api/project/navigations?userId={userId}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<ProjectModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IMultiEnvironmentUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        userContext.SetupGet(user => user.Environment).Returns("development");
        var projectService = new Mock<ProjectService>(caller.Object, userContext.Object);
        var result = await projectService.Object.GetGlobalNavigations();
        caller.Verify(provider => provider.GetAsync<List<ProjectModel>>(requestUri, default), Times.Once);
        Assert.IsTrue(result.Count == 1);
    }
}
