// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Tests;

[TestClass]
public class DefaultRequestMessageTest
{
    [TestMethod]
    public void TestTrySetCulture()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        var requestMessage = new JsonDefaultRequestMessage(services.BuildServiceProvider());

        var httpRequestMessage = new HttpRequestMessage();
        var value = " .AspNetCore.Culture=te";
        httpRequestMessage.Headers.Add("cookie", new List<string?>()
        {
            value
        });
        requestMessage.TestTrySetCulture(httpRequestMessage, new List<(string Key, string Value)>()
        {
            ("c", "en-US"),
            ("uic", "en-US")
        });
        var cookies = httpRequestMessage.Headers.GetValues("cookie").ToList();
        Assert.IsTrue(cookies.Contains(value));
    }
}
