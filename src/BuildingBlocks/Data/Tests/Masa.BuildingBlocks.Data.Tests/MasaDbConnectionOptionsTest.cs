// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.Tests;

[TestClass]
public class MasaDbConnectionOptionsTest
{
    [TestMethod]
    public void TestTryAddConnectionString()
    {
        var masaDbConnectionOptions = new MasaDbConnectionOptions();
        masaDbConnectionOptions.TryAddConnectionString("masa", "Data Source=masa.db;");
        masaDbConnectionOptions.TryAddConnectionString("masa2", "Data Source=masa2.db;");
        masaDbConnectionOptions.TryAddConnectionString("MASA", "Data Source=masa.db;");
        Assert.AreEqual(2, masaDbConnectionOptions.ConnectionStrings.Count);
    }
}
