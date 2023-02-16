// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth.Tests;

[TestClass]
public class LoginServiceTest
{
    private Mock<HttpMessageHandler> _mockHandler = new();
    private LoginService? _loginService;

    [TestInitialize]
    public void Initialized()
    {
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var httpClient = new HttpClient(_mockHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost");
        httpClientFactory.Setup(provider => provider.CreateClient(Constants.DEFAULT_SSO_CLIENT_NAME)).Returns(httpClient);
        _loginService = new LoginService(httpClientFactory.Object);
    }

    [TestMethod]
    public async Task TestLoginByPasswordAsync()
    {
        SetTestData(new { });
        var login = new LoginByPasswordModel
        {
            ClientId = "test_client_id",
            ClientSecret = "test_client_secret",
            Account = "guest",
            Password = "guest123"
        };

        var result = await _loginService!.LoginByPasswordAsync(login);
        Assert.IsNotNull(result);
    }

    private void SetTestData(object result, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
    {
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = httpStatusCode,
               Content = new StringContent(JsonSerializer.Serialize(result))
           });
    }
}

