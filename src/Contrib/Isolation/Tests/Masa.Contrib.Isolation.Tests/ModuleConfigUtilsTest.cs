// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.Tests;

[TestClass]
public class ModuleConfigUtilsTest
{
    [TestMethod]
    public void TestTryGetConfig()
    {
        var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var configuration = configurationBuilder.Build();
        var isolationOptions = configuration.GetSection("Isolation").Get<List<IsolationConfigurationOptions<MasaDbConnectionOptions>>>();
        Assert.IsNotNull(isolationOptions);

        // Assert.AreEqual(1, isolationOptions.Data.Count);
        //
        // var module = isolationOptions.Data[0].Module;
        //
        // var result = ModuleConfigUtils.TryGetConfig<MasaDbConnectionOptions>(
        //     module,
        //     ConnectionStrings.DEFAULT_SECTION,
        //     out var dbConnectionOptions);
        // Assert.IsTrue(result);
        // Assert.IsNotNull(dbConnectionOptions);
        // var connectionString = dbConnectionOptions.ConnectionStrings.GetConnectionString(ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME);
        // Assert.AreEqual("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;", connectionString);
    }
}
