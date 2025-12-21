// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Enums.Tests;

#pragma warning disable CS0618
[TestClass]
public class EnumUtilsTest
{
    [TestMethod]
    public void TestGetSubitemAttribute()
    {
        var attribute = EnumUtil.GetSubitemAttribute<DescriptionAttribute>(Human.Boy);
        Assert.IsNotNull(attribute);
        Assert.AreEqual("BOY", attribute.Description);

        attribute = EnumUtil.GetSubitemAttribute<DescriptionAttribute>(Human.Girl);
        Assert.IsNotNull(attribute);
        Assert.AreEqual("", attribute.Description);
    }

    [TestMethod]
    public void TestGetDescriptionValue()
    {
        var value = EnumUtil.GetDescriptionValue<Human>(1);
        Assert.AreEqual("BOY", value);

        value = EnumUtil.GetDescriptionValue<Human>(2);
        Assert.AreEqual(nameof(Human.Girl), value);

        value = EnumUtil.GetDescriptionValue<Human>(3);
        Assert.AreEqual(null, value);
    }

    [TestMethod]
    public void TestGetDescription()
    {
        var attribute = EnumUtil.GetDescription<Human>(1);
        Assert.IsNotNull(attribute);
        Assert.AreEqual("BOY", attribute.Description);

        attribute = EnumUtil.GetDescription<Human>(2);
        Assert.IsNotNull(attribute);
        Assert.AreEqual(nameof(Human.Girl), attribute.Description);

        attribute = EnumUtil.GetDescription<Human>(3);
        Assert.IsNull(attribute);
    }

    [TestMethod]
    public void TestGetCustomAttribute()
    {
        var attribute = EnumUtil.GetCustomAttribute<Human, DescriptionAttribute>(1);
        Assert.IsNotNull(attribute);
        Assert.AreEqual("BOY", attribute.Description);

        var eName = EnumUtil.GetCustomAttribute<Human, ENameAttribute>(1);
        Assert.IsNotNull(eName);
        Assert.AreEqual("男", eName.Name);

        attribute = EnumUtil.GetCustomAttribute<Human, DescriptionAttribute>(2);
        Assert.IsNotNull(attribute);
        Assert.AreEqual(string.Empty, attribute.Description);

        eName = EnumUtil.GetCustomAttribute<Human, ENameAttribute>(2);
        Assert.IsNotNull(eName);
        Assert.AreEqual(string.Empty, eName.Name);

        attribute = EnumUtil.GetCustomAttribute<Human, DescriptionAttribute>(3);
        Assert.IsNull(attribute);

        eName = EnumUtil.GetCustomAttribute<Human, ENameAttribute>(3);
        Assert.IsNull(eName);

        attribute = EnumUtil.GetCustomAttribute<Human, DescriptionAttribute>(null);
        Assert.IsNull(attribute);

        eName = EnumUtil.GetCustomAttribute<Human, ENameAttribute>(null);
        Assert.IsNull(eName);
    }

    [TestMethod]
    public void TestGetCustomAttributeByFund()
    {
        var attribute = EnumUtil.GetCustomAttribute<Human, DescriptionAttribute>(1, name => new DescriptionAttribute(name + "t"));
        Assert.IsNotNull(attribute);
        Assert.AreEqual("BOY", attribute.Description);

        var eName = EnumUtil.GetCustomAttribute<Human, ENameAttribute>(1, name => new ENameAttribute(name + "t"));
        Assert.IsNotNull(eName);
        Assert.AreEqual("男", eName.Name);

        attribute = EnumUtil.GetCustomAttribute<Human, DescriptionAttribute>(2, name => new DescriptionAttribute(name + "t"));
        Assert.IsNotNull(attribute);
        Assert.AreEqual(nameof(Human.Girl) + "t", attribute.Description);

        eName = EnumUtil.GetCustomAttribute<Human, ENameAttribute>(2, name => new ENameAttribute(name + "t"));
        Assert.IsNotNull(eName);
        Assert.AreEqual(nameof(Human.Girl) + "t", eName.Name);

        attribute = EnumUtil.GetCustomAttribute<Human, DescriptionAttribute>(3, name => new DescriptionAttribute(name + "t"));
        Assert.IsNull(attribute);

        eName = EnumUtil.GetCustomAttribute<Human, ENameAttribute>(3, name => new ENameAttribute(name + "t"));
        Assert.IsNull(eName);
    }

    [TestMethod]
    public void TestGetCustomAttributeDictionary()
    {
        var dictionary = EnumUtil.GetCustomAttributeDictionary<Human, DescriptionAttribute>();
        Assert.IsNotNull(dictionary);
        Assert.AreEqual(2, dictionary.Count);

        Assert.IsTrue(dictionary.ContainsKey(Human.Boy));
        Assert.AreEqual("BOY", dictionary[Human.Boy].Description);
        Assert.IsTrue(dictionary.ContainsKey(Human.Girl));
        Assert.AreEqual(string.Empty, dictionary[Human.Girl].Description);
    }

    [TestMethod]
    public void TestGetItems()
    {
        var list = EnumUtil.GetItems<Human>().ToList();
        Assert.AreEqual(2, list.Count);
        Assert.IsTrue(list.Contains(Human.Boy));
        Assert.IsTrue(list.Contains(Human.Girl));
    }

    [TestMethod]
    public void TestGetList()
    {
        var list = EnumUtil.GetList<Human>().ToList();
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual(1, list[0].Value);
        Assert.AreEqual(2, list[1].Value);
        Assert.AreEqual("BOY", list[0].Name);
        Assert.AreEqual(nameof(Human.Girl), list[1].Name);

        list = EnumUtil.GetList<Human>(true).ToList();
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(1, list[1].Value);
        Assert.AreEqual(2, list[2].Value);
        Assert.AreEqual("BOY", list[1].Name);
        Assert.AreEqual(nameof(Human.Girl), list[2].Name);

        Assert.ThrowsExactly<NotSupportedException>(() => EnumUtil.GetList(typeof(int)));
    }

    [TestMethod]
    public void TestGetEnumList()
    {
        var list = EnumUtil.GetEnumList<Human>().ToList();
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual(Human.Boy, list[0].Value);
        Assert.AreEqual(Human.Girl, list[1].Value);
        Assert.AreEqual("BOY", list[0].Name);
        Assert.AreEqual(nameof(Human.Girl), list[1].Name);

        list = EnumUtil.GetEnumList<Human>(true).ToList();
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(Human.Boy, list[1].Value);
        Assert.AreEqual(Human.Girl, list[2].Value);
        Assert.AreEqual("BOY", list[1].Name);
        Assert.AreEqual(nameof(Human.Girl), list[2].Name);
    }
}

#pragma warning restore CS0618
