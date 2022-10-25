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
        Assert.IsFalse(result);
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
        Assert.IsTrue(result);
    }
}
