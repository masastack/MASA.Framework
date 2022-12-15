// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Enums.Tests;

[TestClass]
public class EnumExtensionsTest
{
    [TestMethod]
    public void TestGetDescription()
    {
        var attribute = Human.Boy.GetDescription();
        Assert.IsNotNull(attribute);
        Assert.AreEqual("BOY", attribute.Description);

        attribute = Human.Girl.GetDescription();
        Assert.IsNotNull(attribute);
        Assert.AreEqual(nameof(Human.Girl), attribute.Description);
    }

    [TestMethod]
    public void TestGetDescriptionValue()
    {
        var value = Human.Boy.GetDescriptionValue();
        Assert.IsNotNull(value);
        Assert.AreEqual("BOY", value);

        value = Human.Girl.GetDescriptionValue();
        Assert.IsNotNull(value);
        Assert.AreEqual(nameof(Human.Girl), value);
    }

    [TestMethod]
    public void TestGetAttribute()
    {
        var attribute = Human.Boy.GetCustomAttribute<DescriptionAttribute>();
        Assert.IsNotNull(attribute);
        Assert.AreEqual("BOY", attribute.Description);

        attribute = Human.Girl.GetCustomAttribute<DescriptionAttribute>();
        Assert.IsNotNull(attribute); //若枚举未增加特性会默认初始化
        Assert.AreEqual(string.Empty, attribute.Description);
    }


    [TestMethod]
    public void TestGetAttributeByFunc()
    {
        var attribute = Human.Boy.GetCustomAttribute(() => new DescriptionAttribute("男"));
        Assert.IsNotNull(attribute);
        Assert.AreEqual("BOY", attribute.Description);

        attribute = Human.Girl.GetCustomAttribute<DescriptionAttribute>(() => new DescriptionAttribute("女"));
        Assert.IsNotNull(attribute);
        Assert.AreEqual("女", attribute.Description);
    }
}
