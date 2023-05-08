// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests;

[TestClass]
public class DefaultIntegrationEventDaprProviderTest
{
    private DefaultIntegrationEventDaprProvider _provider;

    [DataRow("masa", "masa-test", "masa-dev")]
    [DataRow("masa", "", "masa-dev")]
    [DataRow(null, "masa-test", "masa-test")]
    [DataRow(null, "", null)]
    [DataTestMethod]
    public void TestGetDaprAppId(string? appId, string appIdByGlobal, string? exceptedAppId)
    {
        Initialize(appIdByGlobal, () =>
        {
            var actualAppId = _provider.GetDaprAppId(appId);
            Assert.AreEqual(exceptedAppId, actualAppId);
        });
    }

    [DataRow("masa", "masa-test", "masa-dev")]
    [DataRow("masa", "", "masa-dev")]
    [DataRow(null, "masa-test", "masa-env")]
    [DataRow(null, "", "masa-env")]
    [DataTestMethod]
    public void TestGetDaprAppIdBySetDaprAppId(string? appId, string appIdByGlobal, string? exceptedAppId)
    {
        Initialize(appIdByGlobal, () =>
        {
            Environment.SetEnvironmentVariable(DaprStarterConstant.DEFAULT_DAPR_APPID, "masa-env");
            var actualAppId = _provider.GetDaprAppId(appId);
            Assert.AreEqual(exceptedAppId, actualAppId);
            Environment.SetEnvironmentVariable(DaprStarterConstant.DEFAULT_DAPR_APPID, "");
        });
    }

    void Initialize(string appIdByGlobal, Action action)
    {
        Environment.SetEnvironmentVariable("masa", "masa-dev");
        var options = Microsoft.Extensions.Options.Options.Create(new MasaAppConfigureOptions()
        {
            AppId = appIdByGlobal
        });
        _provider = new DefaultIntegrationEventDaprProvider(options);
        action.Invoke();
        Environment.SetEnvironmentVariable("masa", "");
    }
}
