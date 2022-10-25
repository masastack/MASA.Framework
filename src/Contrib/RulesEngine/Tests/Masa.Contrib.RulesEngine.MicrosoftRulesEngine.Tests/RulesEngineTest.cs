// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.RulesEngine.MicrosoftRulesEngine.Tests;

[TestClass]
public class RulesEngineTest
{
    [TestMethod]
    public void TestAddRulesEngine()
    {
        var services = new ServiceCollection();
        services.AddRulesEngine(rulesEngineOptions =>
        {
            rulesEngineOptions.UseMicrosoftRulesEngine();
        });
        var serviceProvider = services.BuildServiceProvider();
        var ruleEngineFactory = serviceProvider.GetService<IRulesEngineFactory>();
        Assert.IsNotNull(ruleEngineFactory);
        var ruleEngineClient = serviceProvider.GetService<IRulesEngineClient>();
        Assert.IsNotNull(ruleEngineClient);
    }

    [TestMethod]
    public void TestAddRulesEngine2()
    {
        var services = new ServiceCollection();
        services.AddRulesEngine(rulesEngineOptions =>
        {
            rulesEngineOptions.UseMicrosoftRulesEngine(new ReSettings()
            {
                CustomTypes = new[] { typeof(StringExtensions) }
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        var ruleEngineClient = serviceProvider.GetService<IRulesEngineClient>();
        Assert.IsNotNull(ruleEngineClient);

        var json = File.ReadAllText(Path.Combine("Rules", "2.json"));
        var result = ruleEngineClient.Execute(json, new
        {
            Name = string.Empty
        });
        Assert.IsFalse(result[0].IsValid);

        json = File.ReadAllText(Path.Combine("Rules", "1.json"));
        result = ruleEngineClient.Execute(json, new
        {
            Age = 19
        });
        Assert.IsTrue(result[0].IsValid);
    }

    [DataRow("", false)]
    [DataRow(null, false)]
    [DataRow("Jim", true)]
    [DataTestMethod]
    public void TestAddRulesEngineAndSpecifyRuleSettings(string? name, bool expectResult)
    {
        var services = new ServiceCollection();
        services.AddRulesEngine(rulesEngineOptions =>
        {
            rulesEngineOptions.UseMicrosoftRulesEngine(new ReSettings()
            {
                CustomTypes = new[] { typeof(StringExtensions) }
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        var ruleEngineClient = serviceProvider.GetService<IRulesEngineClient>();
        Assert.IsNotNull(ruleEngineClient);

        var json = File.ReadAllText(Path.Combine("Rules", "2.json"));
        var result = ruleEngineClient.Execute(json, new
        {
            Name = name
        });
        Assert.AreEqual(expectResult, result[0].IsValid);
    }

    [DataRow("", false)]
    [DataRow(null, false)]
    [DataRow("Jim", true)]
    [DataTestMethod]
    public void TestAddRulesEngineAndSpecifyRuleSettings2(string? name, bool expectResult)
    {
        var services = new ServiceCollection();
        services.AddRulesEngine(rulesEngineOptions =>
        {
            rulesEngineOptions.UseMicrosoftRulesEngine(new ReSettings()
            {
                CustomTypes = new[] { typeof(StringExtensions) }
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        var ruleEngineClient = serviceProvider.GetService<IRulesEngineClient>();
        Assert.IsNotNull(ruleEngineClient);

        var json = File.ReadAllText(Path.Combine("Rules", "4.json"));
        var result = ruleEngineClient.Execute(json, new
        {
            Name = name
        });
        Assert.AreEqual(expectResult, result[0].IsValid);
    }
}
