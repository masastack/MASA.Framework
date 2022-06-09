// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Tests;

[TestClass]
public class SubjectServiceTest : BaseAuthTest
{
    [TestMethod]
    public async Task TestGetListAsync()
    {
        var data = new List<SubjectModel>()
        {
            new SubjectModel()
        };
        string filter = "test";
        var requestUri = $"api/subject/list";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<object, List<SubjectModel>>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var authClient = new AuthClient(callerProvider.Object);
        var result = await authClient.SubjectService.GetListAsync(filter);
        callerProvider.Verify(provider => provider.GetAsync<object, List<SubjectModel>>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result.Count == 1);
    }
}

