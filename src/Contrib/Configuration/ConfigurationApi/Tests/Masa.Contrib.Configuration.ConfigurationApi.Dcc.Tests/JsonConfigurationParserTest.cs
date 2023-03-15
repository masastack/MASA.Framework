// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests;

[TestClass]
public class JsonConfigurationParserTest
{

    [TestMethod]
    public void TestFormatRawByJsonObject()
    {
        var raw = "{\"Id\": 1, \"Name\": \"Jack\"}";
        var result = JsonConfigurationParser.Parse(raw);
        Assert.IsTrue(result["Id"] == "1" && result["Name"] == "Jack");
    }

    [TestMethod]
    public void TestFormatRawByJsonArray()
    {
        var raw = "[{\"Id\": 1, \"Name\": \"Jack\"},{\"Id\": 2, \"Name\": \"Green\"}]";
        var result = JsonConfigurationParser.Parse(raw);
        Assert.AreEqual(4, result.Count);
        Assert.AreEqual("1", result["0:Id"]);
        Assert.AreEqual("Green", result["1:Name"]);
    }
}
