// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.Tests;

[TestClass]
public class MasaConfigurationOptionsBuilderExtensionsTest
{
    [DataRow(true, false, 1)]
    [DataRow(true, true, 2)]
    [DataRow(false, true, 1)]
    [DataRow(false, false, 0)]
    [DataTestMethod]
    public void TestBuildOptionsRelations(bool enableAutoMapOptions, bool isManualMapping, int expectedNum)
    {
        var services = new ServiceCollection();
        var masaConfigurationOptionsBuilder = new MasaConfigurationOptionsBuilder(services)
        {
            Assemblies = new List<Assembly>()
            {
                typeof(KafkaOptions).Assembly
            },
            EnableAutoMapOptions = enableAutoMapOptions
        };
        if (isManualMapping)
        {
            masaConfigurationOptionsBuilder.UseMasaOptions(options =>
            {
                options.MappingLocal<RabbitMqOptions>();
            });
        }
        var registrationOptions = masaConfigurationOptionsBuilder.BuildOptionsRelations();

        Assert.AreEqual(expectedNum, registrationOptions.Count);
    }

    [TestMethod]
    public void TestAutoMappingByParameterlessConstructor()
    {
        var services = new ServiceCollection();
        var masaConfigurationOptionsBuilder = new MasaConfigurationOptionsBuilder(services)
        {
            Assemblies = new List<Assembly>()
            {
                typeof(EsFactoryOptions).Assembly
            }
        };

        Assert.ThrowsException<MasaException>(() =>
        {
            masaConfigurationOptionsBuilder.AddOptions();
        });
    }

    [TestMethod]
    public void TestAddOptions()
    {
        var services = new ServiceCollection();
        var masaConfigurationOptionsBuilder = new MasaConfigurationOptionsBuilder(services)
        {
            Assemblies = new[]
            {
                typeof(KafkaOptions).Assembly
            }
        };
        masaConfigurationOptionsBuilder.AddOptions();
        Assert.IsTrue(masaConfigurationOptionsBuilder.Services.Any(d => d.ServiceType == typeof(IConfigureNamedOptions<KafkaOptions>)));
        Assert.IsTrue(masaConfigurationOptionsBuilder.Services.Any(d => d.ServiceType == typeof(IConfigureOptions<KafkaOptions>)));
    }
}
