// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore.Tests;

[TestClass]
public class DefaultIsolationConnectionStringProviderTest
{
    [TestMethod]
    public void TestGetConnectionString()
    {
        var name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME;
        var connectionString = "customConnectionString";
        Mock<IUnitOfWorkAccessor> unitOfWorkAccessor = new();
        var masaDbContextConfigurationOptions = new MasaDbContextConfigurationOptions();
        masaDbContextConfigurationOptions.AddConnectionString(name, connectionString);
        unitOfWorkAccessor.Setup(accessor => accessor.CurrentDbContextOptions).Returns(() => masaDbContextConfigurationOptions);

        Mock<IConnectionStringProviderWrapper> connectionStringProviderWrapper = new();
        Mock<IIsolationConfigProvider> isolationConfigProvider = new();
        var isolationConnectionStringProvider = new DefaultIsolationConnectionStringProvider(
            connectionStringProviderWrapper.Object,
            isolationConfigProvider.Object,
            unitOfWorkAccessor.Object);
        Assert.AreEqual(connectionString, isolationConnectionStringProvider.GetConnectionString(name));

        unitOfWorkAccessor.Setup(accessor => accessor.CurrentDbContextOptions).Returns(() => new MasaDbContextConfigurationOptions());
        var connectionStrings = new ConnectionStrings(new List<KeyValuePair<string, string>>()
        {
            new(name, $"{connectionString}2")
        });
        isolationConfigProvider.Setup(provider => provider.GetModuleConfig<ConnectionStrings>(name, "")).Returns(connectionStrings);

        var isolationConnectionStringProvider2 = new DefaultIsolationConnectionStringProvider(
            connectionStringProviderWrapper.Object,
            isolationConfigProvider.Object,
            unitOfWorkAccessor.Object);
        Assert.AreEqual($"{connectionString}2", isolationConnectionStringProvider2.GetConnectionString(name));
    }
}
