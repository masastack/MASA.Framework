// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.RulesEngine.MicrosoftRulesEngine.Tests;

[TestClass]
public class RulesEngineClientTest
{
    [TestMethod]
    public void TestExecute()
    {
        var rulesEngineClient = new RulesEngineClient();
        var json = File.ReadAllText(Path.Combine("Rules", "1.json"));
        var result = rulesEngineClient.Execute(json, new
        {
            Age = 16
        });
        Assert.IsFalse(result[0].IsValid);
    }

    [TestMethod]
    public async Task TestExecuteAsync()
    {
        var rulesEngineClient = new RulesEngineClient();
        var json = await File.ReadAllTextAsync(Path.Combine("Rules", "1.json"));
        var result = await rulesEngineClient.ExecuteAsync(json, new
        {
            Age = 19
        });
        Assert.IsTrue(result[0].IsValid);
    }

    [DataRow("3.json", false)]
    [DataRow("1.json", true)]
    [DataTestMethod]
    public void TestVerify(string fileName, bool expectResult)
    {
        var rulesEngineClient = new RulesEngineClient();
        var json = File.ReadAllText(Path.Combine("Rules", fileName));
        var result = rulesEngineClient.Verify(json);
        Assert.AreEqual(expectResult, result.IsValid);
    }

    [DataRow("3.json", false)]
    [DataRow("1.json", true)]
    [DataTestMethod]
    public void TestVerifyAndUseLogger(string fileName, bool expectResult)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var serviceProvider = services.BuildServiceProvider();
        var rulesEngineClient = new RulesEngineClient(logger: serviceProvider.GetRequiredService<ILogger<RulesEngineClient>>());
        var json = File.ReadAllText(Path.Combine("Rules", fileName));
        var result = rulesEngineClient.Verify(json);
        Assert.AreEqual(expectResult, result.IsValid);
    }
}
