// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DotNet.Tests;

[TestClass]
public class AttributeTest
{
    [TestMethod]
    public void TestGetDescriptionByConst()
    {
        var value = AttributeUtils.GetDescriptionByConst(typeof(ErrorCode), nameof(ErrorCode.FRAMEWORK_PREFIX));
        Assert.AreEqual("Framework Prefix", value);
    }
}

public static class ErrorCode
{
    [System.ComponentModel.Description("Framework Prefix")]
    public const string FRAMEWORK_PREFIX = "MF";
}
