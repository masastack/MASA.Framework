// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Enums.Tests;

#pragma warning disable CS0618
[TestClass]
public class EnumTest
{
    [TestMethod]
    public void TestGetAttributes()
    {
        var list = Enum<Human>.GetAttributes<DescriptionAttribute>();
        Assert.IsNotNull(list);
        Assert.AreEqual(2, list.Count);
    }
}
#pragma warning restore CS0618
