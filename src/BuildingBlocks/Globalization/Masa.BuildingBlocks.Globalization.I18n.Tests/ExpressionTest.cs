// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Tests;

[TestClass]
public class ExpressionTest
{
    [TestMethod]
    public void TestGetI18nName()
    {
        var name = ExpressionExtensions.GetI18nName<CustomResource>(c => c.Name);
        Assert.AreEqual("Name", name);

        name = ExpressionExtensions.GetI18nName<CustomResource>(c => c.Order.Name);
        Assert.AreEqual("Order.Name", name);
    }
}
