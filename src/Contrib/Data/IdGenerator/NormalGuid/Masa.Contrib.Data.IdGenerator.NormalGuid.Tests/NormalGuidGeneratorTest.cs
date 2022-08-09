// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.NormalGuid.Tests;

[TestClass]
public class NormalGuidGeneratorTest
{
    [TestMethod]
    public void TestNormarlGuidReturnIdGeneratorIsNotNull()
    {
        var services = new ServiceCollection();
        services.AddSimpleGuidGenerator();
        var serviceProvider = services.BuildServiceProvider();
        var idGenerator = serviceProvider.GetService<IIdGenerator<Guid>>();
        Assert.IsNotNull(idGenerator);
        Assert.IsTrue(idGenerator.GetType() == typeof(NormalGuidGenerator));

        Assert.IsNotNull(serviceProvider.GetService<IIdGenerator>());
    }

    [TestMethod]
    public void TestNormarlGuidByMasaAppReturnIdGeneratorIsNotNull()
    {
        var services = new ServiceCollection();
        MasaApp.Services = services;
        services.AddSimpleGuidGenerator();

        var idGenerator = MasaApp.GetService<IIdGenerator<Guid>>();
        Assert.IsNotNull(idGenerator);
        Assert.IsTrue(idGenerator.GetType() == typeof(NormalGuidGenerator));

        Assert.IsNotNull(MasaApp.GetService<IIdGenerator>());
    }
}
