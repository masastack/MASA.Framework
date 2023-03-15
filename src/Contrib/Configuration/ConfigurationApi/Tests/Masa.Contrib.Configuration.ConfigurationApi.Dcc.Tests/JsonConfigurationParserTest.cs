// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal.Parser;

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests;

[TestClass]
public class JsonConfigurationParserTest
{

    [TestMethod]
    public void TestFormatRawByJsonObject()
    {
        var raw = "{\"Id\": 1, \"Name\": \"Jack\"}";
        var result = JsonConfigurationParser.Parse(raw);
        Assert.IsTrue(result.Count > 0);
    }

    [TestMethod]
    public void TestFormatRawByJsonArray()
    {
        var raw = "[{\"Id\": 1, \"Name\": \"Jack\"},{\"Id\": 2, \"Name\": \"Green\"}]";
        var result = JsonConfigurationParser.Parse(raw);
        Assert.IsTrue(result.Count > 0);
    }
}
