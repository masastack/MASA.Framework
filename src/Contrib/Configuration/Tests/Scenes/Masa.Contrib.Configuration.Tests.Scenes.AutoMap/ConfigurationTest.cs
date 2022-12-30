﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.Tests.Scenes.AutoMap;

#pragma warning disable CS0618
[TestClass]
public class ConfigurationTest
{
    [TestMethod]
    public void TestAddMasaConfiguration()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddMasaConfiguration();
        var serviceProvider = builder.Services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<KafkaOptions>>();
        Assert.IsNotNull(options);
        Assert.AreEqual("Kafka Server", options.Value.Servers);
        Assert.AreEqual(10, options.Value.ConnectionPoolSize);
    }
}
#pragma warning restore CS0618
