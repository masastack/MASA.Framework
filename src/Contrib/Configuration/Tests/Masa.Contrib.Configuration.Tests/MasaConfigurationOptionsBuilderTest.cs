// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.Tests;

[TestClass]
public class MasaConfigurationOptionsBuilderTest
{
    [DataRow("ParentSection", SectionTypes.Local, "Section", "", typeof(string), true)]
    [DataRow("Parent", SectionTypes.Local, "Section", "", typeof(string), false)]
    [DataRow("ParentSection", SectionTypes.ConfigurationApi, "Section", "", typeof(string), false)]
    [DataRow("ParentSection", SectionTypes.Local, "Section", "name", typeof(string), false)]
    [DataRow("ParentSection", SectionTypes.Local, "Section", "", typeof(int), false)]
    [DataTestMethod]
    public void TestMasaConfigurationOptionsBuilder(
        string parentSection,
        SectionTypes sectionType,
        string section,
        string name,
        Type objectType,
        bool expectedException)
    {
        var services = new ServiceCollection();
        var masaConfigurationOptionsBuilder = new MasaConfigurationOptionsBuilder(services);
        masaConfigurationOptionsBuilder.AddRegistrationOptions(new ConfigurationRelationOptions()
        {
            ParentSection = "ParentSection",
            SectionType = SectionTypes.Local,
            Section = "Section",
            Name = Options.DefaultName,
            ObjectType = typeof(string)
        });
        var options = new ConfigurationRelationOptions()
        {
            ParentSection = parentSection,
            SectionType = sectionType,
            Section = section,
            Name = name,
            ObjectType = objectType
        };

        if (expectedException)
        {
            Assert.ThrowsException<MasaException>(() =>
            {
                masaConfigurationOptionsBuilder.AddRegistrationOptions(options);
            });
        }
        else
        {
            masaConfigurationOptionsBuilder.AddRegistrationOptions(options);
            Assert.AreEqual(2, masaConfigurationOptionsBuilder.RegistrationOptions.Count);
        }
    }
}
