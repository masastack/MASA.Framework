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
        services.TestAddSimpleGuidGenerator();
        var idGenerator = MasaApp.GetService<IIdGenerator<Guid>>();
        Assert.IsNotNull(idGenerator);
        Assert.IsTrue(idGenerator.GetType() == typeof(NormalGuidGenerator));

        Assert.IsNotNull(MasaApp.GetService<IIdGenerator>());
    }

    [TestMethod]
    public void TestNormarlGuidByCustomNameReturnIdGeneratorIsNotNull()
    {
        var services = new ServiceCollection();
        services.TestAddSimpleGuidGenerator("normal");
        var idGeneratorFactory = MasaApp.GetService<IIdGeneratorFactory>();
        Assert.IsNotNull(idGeneratorFactory);

        var idGenerator = idGeneratorFactory.Create("normal");
        Assert.IsNotNull(idGenerator);
        Assert.IsTrue(idGenerator.GetType() == typeof(NormalGuidGenerator));
    }

    [TestMethod]
    public void TestNormarlGuidByNameIsNullReturnArgumentNullException()
    {
        var services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => services.TestAddSimpleGuidGenerator(null!));
    }

    [TestMethod]
    public void TestAddMultiSequentialGuidReturnIdGeneratorCountIs1()
    {
        var services = new ServiceCollection();
        services.TestAddSimpleGuidGenerator().TestAddSimpleGuidGenerator();

        Assert.IsTrue(services.Count(d => d.ServiceType == typeof(IIdGenerator<Guid>)) == 1);
    }

    [TestMethod]
    public void TestNewId()
    {
        var services = new ServiceCollection();
        services.TestAddSimpleGuidGenerator();

        Assert.AreNotEqual(Guid.Empty, MasaApp.GetRequiredService<IIdGenerator<Guid>>().NewId());
    }
}
