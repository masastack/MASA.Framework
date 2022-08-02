// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.StackSdks.Auth.Service;

namespace Masa.Contrib.StackSdks.Auth.Tests;

[TestClass]
public class SubjectServiceTest
{
    [TestMethod]
    public async Task TestGetListAsync()
    {
        var data = new List<SubjectModel>()
        {
            new SubjectModel()
        };
        string filter = "test";
        var requestUri = $"api/subject/getList";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, List<SubjectModel>>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var subjectService = new Mock<SubjectService>(caller.Object);
        var result = await subjectService.Object.GetListAsync(filter);
        caller.Verify(provider => provider.GetAsync<object, List<SubjectModel>>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result.Count == 1);
    }
}

