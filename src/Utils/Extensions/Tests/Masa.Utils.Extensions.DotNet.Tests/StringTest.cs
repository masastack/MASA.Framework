// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DotNet.Tests;

[TestClass]
public class StringTest
{
    [DataTestMethod]
    [DataRow("abcdawDf", "abc", "dawDf")]
    [DataRow("abcdawDf", "abd", "abcdawDf")]
    public void TestTrimStart(string value, string trimParameter, string result)
    {
        Assert.AreEqual(result, value.TrimStart(trimParameter, StringComparison.OrdinalIgnoreCase));
    }

    [DataTestMethod]
    [DataRow("abcdawDf", "df", "abcdaw")]
    [DataRow("abcdawDf", "adf", "abcdawDf")]
    public void TestTrimEnd(string value, string trimParameter, string result)
    {
        Assert.AreEqual(result, value.TrimEnd(trimParameter, StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void Test()
    {

    }
}
