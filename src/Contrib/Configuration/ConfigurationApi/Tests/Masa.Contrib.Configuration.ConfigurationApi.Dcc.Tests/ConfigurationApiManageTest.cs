// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests;

[TestClass]
public class ConfigurationApiManageTest
{
    private DccConfigurationOptions _dccConfigurationOptions;
    private JsonSerializerOptions _jsonSerializerOptions;
    private Mock<ICaller> _caller;

    [TestInitialize]
    public void Initialize()
    {
        _dccConfigurationOptions = new()
        {
            DefaultSection = new DccSectionOptions()
            {
                Environment = "Test",
                Cluster = "Default",
                AppId = "DccTest",
                ConfigObjects = new List<string>()
                {
                    "Test1"
                },
                Secret = "Secret"
            }
        };
        _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        _caller = new Mock<ICaller>();
    }

    [DataTestMethod]
    [DataRow(HttpStatusCode.ExpectationFailed, "error", true)]
    [DataRow(299, "custom error", true)]
    [DataRow(200, "succeed", false)]
    public async Task TestUpdateAsync(int statusCode, string assignContent, bool isThrowException)
    {
        var environment = "Test";
        var cluster = "Default";
        var appId = "DccTest";
        var configObject = "Brand";
        var brand = new Brands("Microsoft");

        _caller.Setup(factory => factory.PutAsync(It.IsAny<string>(), It.IsAny<object>(), false, default).Result).Returns(()
            => new HttpResponseMessage()
            {
                StatusCode = (HttpStatusCode)statusCode,
                Content = new StringContent(assignContent)
            }).Verifiable();

        var manage = new ConfigurationApiManage(_caller.Object, _jsonSerializerOptions, _dccConfigurationOptions);
        if (isThrowException)
        {
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async ()
                => await manage.UpdateAsync(environment, cluster, appId, configObject, brand));
        }
        else
        {
            await manage.UpdateAsync(environment, cluster, appId, configObject, brand);
        }
    }

    [DataTestMethod]
    [DataRow(HttpStatusCode.ExpectationFailed, "error", true)]
    [DataRow(299, "custom error", true)]
    [DataRow(200, "succeed", false)]
    public async Task TestAddAsync(int statusCode, string assignContent, bool isThrowException)
    {
        var environment = "Test";
        var cluster = "Default";
        var appId = "DccTest";
        var configObjects = new Dictionary<string, object>
        {
            {
                "Appsettings", new
                {
                    ConnectionStrings = "xxxx"
                }
            }
        };

        _caller.Setup(factory => factory.PostAsync(It.IsAny<string>(), It.IsAny<object>(), false, default).Result).Returns(()
            => new HttpResponseMessage()
            {
                StatusCode = (HttpStatusCode)statusCode,
                Content = new StringContent(assignContent)
            }).Verifiable();

        var manage = new ConfigurationApiManage(_caller.Object, _jsonSerializerOptions, _dccConfigurationOptions);
        if (isThrowException)
        {
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async ()
                => await manage.AddAsync(environment, cluster, appId, configObjects));
        }
        else
        {
            await manage.AddAsync(environment, cluster, appId, configObjects);
        }
    }
}
