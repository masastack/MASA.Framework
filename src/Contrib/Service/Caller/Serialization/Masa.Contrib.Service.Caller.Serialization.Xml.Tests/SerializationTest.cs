// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Serialization.Xml.Tests;

[TestClass]
public class SerializationTest
{
    private User _user;

    [TestInitialize]
    public void Initialize()
    {
        _user = new User()
        {
            Name = "masa",
            Age = 18
        };
    }

    [TestMethod]
    public async Task TestAsync()
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "");
        var xmlRequestMessage = new XmlRequestMessage();
        xmlRequestMessage.ProcessHttpRequestMessage(requestMessage, _user);

        Assert.IsNotNull(requestMessage.Content);

        var xmlResponseMessage = new CustomXmlResponseMessage();
        var actualUser = await xmlResponseMessage.GetCustomResponseAsync<User>(requestMessage.Content, default);
        Assert.IsNotNull(actualUser);
        Assert.AreEqual(_user.Name, actualUser.Name);
        Assert.AreEqual(_user.Age, actualUser.Age);
    }
}
