// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.Tests;

[TestClass]
public class ConnectionStringNameTest
{
    [TestMethod]
    public void TestGetConnectionStringName()
    {
        string name = ConnectionStringNameAttribute.GetConnStringName<CustomDbContext>();
        Assert.AreEqual(ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, name);

        name = ConnectionStringNameAttribute.GetConnStringName<CustomQueryDbContext>();
        Assert.AreEqual(typeof(CustomQueryDbContext).FullName, name);

        name = ConnectionStringNameAttribute.GetConnStringName<CustomQuery2DbContext>();
        Assert.AreEqual(typeof(CustomQuery2DbContext).FullName, name);

        name = ConnectionStringNameAttribute.GetConnStringName<CustomQuery3DbContext>();
        Assert.AreEqual("query3", name);
    }
}
