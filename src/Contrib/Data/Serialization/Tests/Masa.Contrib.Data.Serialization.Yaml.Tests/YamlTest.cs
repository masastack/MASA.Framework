// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Serialization.Yaml.Tests;

[TestClass]
public class YamlTest
{
    [TestMethod]
    public void TestSerializeAndDeserialize()
    {
        var user = new User(1.5m, "John");
        var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        var yaml = new DefaultYamlSerializer(serializer).Serialize(user);
        var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        var deserializerUser = new DefaultYamlDeserializer(deserializer).Deserialize<User>(yaml);
        Assert.IsNotNull(deserializerUser);
        Assert.IsTrue(user.Age == deserializerUser.Age && user.Name == deserializerUser.Name);

        deserializerUser = new DefaultYamlDeserializer(deserializer).Deserialize(yaml, typeof(User)) as User;
        Assert.IsNotNull(deserializerUser);
        Assert.IsTrue(user.Age == deserializerUser.Age && user.Name == deserializerUser.Name);
    }

    [TestMethod]
    public void TestSerializeAndValueIsNullReturnEmpty()
    {
        var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        object? user = null;
        var yaml = new DefaultYamlSerializer(serializer).Serialize(user);
        Assert.AreEqual(string.Empty, yaml);
    }

    [TestMethod]
    public void TestAddYamlReturnNotNull()
    {
        var services = new ServiceCollection();
        services.AddYaml();
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<ISerializer>());
        Assert.IsNotNull(serviceProvider.GetService<IDeserializer>());

        var serializer = serviceProvider.GetService<ISerializer>();
        Assert.IsNotNull(serializer);
        Assert.IsTrue(serializer.GetType() == typeof(DefaultYamlSerializer));

        var deserializer = serviceProvider.GetService<IDeserializer>();
        Assert.IsNotNull(deserializer);
        Assert.IsTrue(deserializer.GetType() == typeof(DefaultYamlDeserializer));
    }

    [TestMethod]
    public void TestAddYamlByUseMasaAppReturnNotNull()
    {
        var services = new ServiceCollection();
        MasaApp.Services = services;
        services.AddYaml();
        MasaApp.Build();
        Assert.IsNotNull(MasaApp.GetService<ISerializer>());
        Assert.IsNotNull(MasaApp.GetService<IDeserializer>());

        var serializer = MasaApp.GetService<ISerializer>();
        Assert.IsNotNull(serializer);
        Assert.IsTrue(serializer.GetType() == typeof(DefaultYamlSerializer));

        var deserializer = MasaApp.GetService<IDeserializer>();
        Assert.IsNotNull(deserializer);
        Assert.IsTrue(deserializer.GetType() == typeof(DefaultYamlDeserializer));
    }

    [TestMethod]
    public void TestAddYamlByUseMasaAppReturnNotNull2()
    {
        var services = new ServiceCollection();
        MasaApp.Services = services;
        services.AddYaml(options =>
        {
            options.Serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            options.Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        });
        MasaApp.Build();
        Assert.IsNotNull(MasaApp.GetService<ISerializer>());
        Assert.IsNotNull(MasaApp.GetService<IDeserializer>());

        var serializer = MasaApp.GetService<ISerializer>();
        Assert.IsNotNull(serializer);
        Assert.IsTrue(serializer.GetType() == typeof(DefaultYamlSerializer));

        var deserializer = MasaApp.GetService<IDeserializer>();
        Assert.IsNotNull(deserializer);
        Assert.IsTrue(deserializer.GetType() == typeof(DefaultYamlDeserializer));
    }

    [TestMethod]
    public void TestAddMultiYamlReturnCountIs1()
    {
        var services = new ServiceCollection();
        services.AddYaml().AddYaml();
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IYamlSerializer>().Count() == 1);
        Assert.IsTrue(serviceProvider.GetServices<IYamlDeserializer>().Count() == 1);
    }
}
