// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Tsc.Tests.Extensions;

[TestClass]
public class CallerProviderExtensionsTests
{
    [TestMethod]
    public async Task GetByBodyAsyncTest()
    {
        var callerProvider = new Mock<ICallerProvider>();
        string url = "http://locahost:80/test";
        var param = new { name = "name" };
        var result = "ok";
        callerProvider.Setup(provider => provider.SendAsync<string>(It.IsAny<HttpRequestMessage>(), default)).ReturnsAsync(result);
        var str = await callerProvider.Object.GetByBodyAsync<string>(url, "name");
        Assert.IsNotNull(str);
        Assert.AreEqual(result, str);
    }
}
