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
    public void TestGetDaprAppId(string? daprAppId, string appIdByGlobal, string? exceptedDaprAppId)
    {
        Initialize(() =>
        {
            var actualDaprAppId = _provider.GetDaprAppId(daprAppId, appIdByGlobal);
            Assert.AreEqual(exceptedDaprAppId, actualDaprAppId);
        });
    }

    [DataRow("masa", "masa-test", "masa-dev")]
    [DataRow("masa", "", "masa-dev")]
    [DataRow(null, "masa-test", "masa-env")]
    [DataRow(null, "", "masa-env")]
    [DataTestMethod]
    public void TestGetDaprAppIdBySetDaprAppId(string? daprAppId, string appIdByGlobal, string? exceptedDaprAppId)
    {
        Initialize(() =>
        {
            Environment.SetEnvironmentVariable(DaprStarterConstant.DEFAULT_DAPR_APPID, "masa-env");
            var actualAppId = _provider.GetDaprAppId(daprAppId, appIdByGlobal);
            Assert.AreEqual(exceptedDaprAppId, actualAppId);
            Environment.SetEnvironmentVariable(DaprStarterConstant.DEFAULT_DAPR_APPID, "");
        });
    }

    void Initialize(Action action)
    {
        Environment.SetEnvironmentVariable("masa", "masa-dev");
        _provider = new DefaultIntegrationEventDaprProvider();
        action.Invoke();
        Environment.SetEnvironmentVariable("masa", "");
    }
}
