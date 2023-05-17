// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.Tests;

[TestClass]
public class MasaConfigurationRelationOptionsTest
{
    private List<ConfigurationRelationOptions> RelationOptionsList { get; set; }
    private MasaConfigurationRelationOptions MasaConfigurationRelationOptions { get; set; }

    [TestInitialize]
    public void Initialize()
    {
        RelationOptionsList = new();
        MasaConfigurationRelationOptions = new MasaConfigurationRelationOptions(RelationOptionsList);
    }

    [DataRow("", "", "", "")]
    [DataRow(null, "RabbitMqOptions", "", "")]
    [DataRow(null, "RabbitMqOptions", "test", "test")]
    [DataRow("RabbitMq", "RabbitMq", "test", "test")]
    [DataTestMethod]
    public void TestMappingLocal(
        string? section, string? expectedSection,
        string name, string expectedName)
    {
        Assert.AreEqual(0, RelationOptionsList.Count);
        MasaConfigurationRelationOptions.MappingLocal<RabbitMqOptions>(section, name);
        Assert.AreEqual(1, RelationOptionsList.Count);

        Assert.AreEqual(typeof(RabbitMqOptions), RelationOptionsList[0].ObjectType);
        Assert.AreEqual(false, RelationOptionsList[0].IsRequiredConfigComponent);
        Assert.AreEqual(SectionTypes.Local, RelationOptionsList[0].SectionType);
        Assert.AreEqual(null, RelationOptionsList[0].ParentSection);
        Assert.AreEqual(expectedName, RelationOptionsList[0].OptionsName);
        Assert.AreEqual(expectedSection, RelationOptionsList[0].Section);
    }

    [DataRow("", null, true, "", "", "", "")]
    [DataRow(null, null, true, "", "", "", "")]
    [DataRow("appid", "appid", false, "", "", "", "")]
    [DataRow("appid", "appid", false, null, "RabbitMqOptions", "", "")]
    [DataRow("appid", "appid", false, "RabbitMq", "RabbitMq", "", "")]
    [DataRow("appid", "appid", false, null, "RabbitMqOptions", "test", "test")]
    [DataTestMethod]
    public void TestMappingConfigurationApi(
        string appId, string expectedParentSection, bool expectedThrowException,
        string? section, string? expectedSection,
        string name, string expectedName)
    {
        Assert.AreEqual(0, RelationOptionsList.Count);
        if (expectedThrowException)
        {
            Assert.ThrowsException<MasaArgumentException>(() =>
            {
                MasaConfigurationRelationOptions.MappingConfigurationApi<RabbitMqOptions>(appId, section, name);
            });
            return;
        }
        MasaConfigurationRelationOptions.MappingConfigurationApi<RabbitMqOptions>(appId, section, name);
        Assert.AreEqual(1, RelationOptionsList.Count);

        Assert.AreEqual(typeof(RabbitMqOptions), RelationOptionsList[0].ObjectType);
        Assert.AreEqual(false, RelationOptionsList[0].IsRequiredConfigComponent);
        Assert.AreEqual(SectionTypes.ConfigurationApi, RelationOptionsList[0].SectionType);
        Assert.AreEqual(expectedParentSection, RelationOptionsList[0].ParentSection);
        Assert.AreEqual(expectedName, RelationOptionsList[0].OptionsName);
        Assert.AreEqual(expectedSection, RelationOptionsList[0].Section);
    }
}
