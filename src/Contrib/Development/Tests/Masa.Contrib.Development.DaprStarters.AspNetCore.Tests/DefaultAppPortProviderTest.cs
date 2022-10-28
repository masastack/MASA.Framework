// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarters.AspNetCore.Tests;

[TestClass]
public class DefaultAppPortProviderTest
{
    [TestMethod]
    public void TestGetAppPort()
    {
        var service = new Mock<IServer>();
        IServerAddressesFeature serverAddressesFeature = new ServerAddressesFeature();
        service.Setup(s => s.Features.Get<IServerAddressesFeature>()).Returns(() => serverAddressesFeature).Verifiable();
        var provider = new DefaultAppPortProvider(service.Object);
        Assert.ThrowsException<UserFriendlyException>(() => provider.GetAppPort(true),
            "Failed to get the startup port, please specify the port manually");
    }

    [DataTestMethod]
    [DataRow(null, false, 5000)]
    [DataRow(false, false, 5000)]
    [DataRow(true, true, 5001)]
    public void TestGetAppPort2(bool? enableSsl, bool expectedEnableSsl, int expectedAppPort)
    {
        var service = new Mock<IServer>();
        IServerAddressesFeature serverAddressesFeature = new ServerAddressesFeature();
        serverAddressesFeature.Addresses.Add("https://localhost:5001");
        serverAddressesFeature.Addresses.Add("http://localhost:5000");
        service.Setup(s => s.Features.Get<IServerAddressesFeature>()).Returns(() => serverAddressesFeature).Verifiable();
        var provider = new DefaultAppPortProvider(service.Object);
        var result = provider.GetAppPort(enableSsl);
        Assert.AreEqual(expectedAppPort, result.AppPort);
        Assert.AreEqual(expectedEnableSsl, result.EnableSsl);
    }

    [DataTestMethod]
    [DataRow(null, true, 5001)]
    [DataRow(true, true, 5001)]
    [DataRow(false, true, 5001)]
    public void TestGetAppPort3(bool? enableSsl, bool expectedEnableSsl, int expectedAppPort)
    {
        var service = new Mock<IServer>();
        IServerAddressesFeature serverAddressesFeature = new ServerAddressesFeature();
        serverAddressesFeature.Addresses.Add("https://localhost:5001");
        service.Setup(s => s.Features.Get<IServerAddressesFeature>()).Returns(() => serverAddressesFeature).Verifiable();
        var provider = new DefaultAppPortProvider(service.Object);
        var result = provider.GetAppPort(enableSsl);
        Assert.AreEqual(expectedAppPort, result.AppPort);
        Assert.AreEqual(expectedEnableSsl, result.EnableSsl);
    }

    [TestMethod]
    public void TestGetAppPort5()
    {
        List<ValueTuple<string, int>> ports = new()
        {
            (Uri.UriSchemeHttps, 5001),
            (Uri.UriSchemeHttp, 5000),
        };
        var port = DefaultAppPortProvider.GetAppPort(ports, true);
        Assert.AreEqual(5001, port);

        port = DefaultAppPortProvider.GetAppPort(ports, false);
        Assert.AreEqual(5000, port);
    }
}
