// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DotNet.Tests;

#pragma warning disable CS0618
[TestClass]
public class AttributeTest
{
    [TestMethod]
    public void TestGetDescriptionByConst()
    {
        var value = AttributeUtils.GetDescriptionByConst(typeof(ErrorCode), nameof(ErrorCode.FRAMEWORK_PREFIX));
        Assert.AreEqual("Framework Prefix", value);

        value = AttributeUtils.GetDescriptionByConst<TestErrorCode>(nameof(ErrorCode.FRAMEWORK_PREFIX));
        Assert.AreEqual("Test Framework Prefix", value);

        value = AttributeUtils.GetDescriptionByField<TestErrorCode>("masa");
        Assert.AreEqual(null, value);
    }

    [TestMethod]
    public void TestGetCustomAttribute()
    {
        var attribute = AttributeUtils.GetCustomAttribute<TestErrorCode, System.ComponentModel.DescriptionAttribute>(nameof(TestErrorCode.FRAMEWORK_PREFIX));
        Assert.IsNotNull(attribute);
        Assert.AreEqual("Test Framework Prefix", attribute.Description);

        attribute = AttributeUtils.GetCustomAttribute<TestErrorCode, System.ComponentModel.DescriptionAttribute>(nameof(TestErrorCode.FRAMEWORK_SUFFIX),out bool existFieldInfo);
        Assert.IsTrue(existFieldInfo);
        Assert.IsNull(attribute);

        attribute = AttributeUtils.GetCustomAttribute<TestErrorCode, System.ComponentModel.DescriptionAttribute>("test",out existFieldInfo);
        Assert.IsFalse(existFieldInfo);
        Assert.IsNull(attribute);
    }
}

public static class ErrorCode
{
    [System.ComponentModel.Description("Framework Prefix")]
    public const string FRAMEWORK_PREFIX = "MF";
}

public class TestErrorCode
{
    [System.ComponentModel.Description("Test Framework Prefix")]
    public const string FRAMEWORK_PREFIX = "MF";

    public const string FRAMEWORK_SUFFIX = "MF_Suffix";
}
#pragma warning restore CS0618
