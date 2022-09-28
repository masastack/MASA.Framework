// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DotNet.Tests;

[TestClass]
public class JsonSerializerTest
{
    [TestMethod]
    public void TestEnableDynamicTypes()
    {
        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.EnableDynamicTypes();
        Assert.AreEqual(1, jsonSerializerOptions.Converters.Count);

        jsonSerializerOptions.EnableDynamicTypes();
        Assert.AreEqual(1, jsonSerializerOptions.Converters.Count);
    }
}
