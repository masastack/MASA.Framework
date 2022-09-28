// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.Cache.Tests;

[TestClass]
public class MapperTest
{
    [TestMethod]
    public void TestToModel()
    {
        var apiScope = new ApiScope("add email", "add email", "add email", true, true, true, true);
        var model = apiScope.ToModel();
        var result = (model.Name, model.DisplayName, model.Description, model.Required,
                      model.Emphasize, model.ShowInDiscoveryDocument, model.Enabled)
                   ==
                   (apiScope.Name, apiScope.DisplayName, apiScope.Description, apiScope.Required,
                      apiScope.Emphasize, apiScope.ShowInDiscoveryDocument, apiScope.Enabled);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestToModel2()
    {
        var identityResource = new IdentityResource("profile", "profile", "profile", true, true, true, true, true);
        var model = identityResource.ToModel();
        var result = (model.Name, model.DisplayName, model.Description, model.Required,
                      model.Emphasize, model.ShowInDiscoveryDocument, model.Enabled)
                   ==
                   (identityResource.Name, identityResource.DisplayName, identityResource.Description, identityResource.Required,
                      identityResource.Emphasize, identityResource.ShowInDiscoveryDocument, identityResource.Enabled);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestToModel3()
    {
        var apiResource = new ApiResource("emai", "email", "email", "", true, default, true, true);
        var model = apiResource.ToModel();
        var result = (model.Name, model.DisplayName, model.Description, string.Join(',', model.AllowedAccessTokenSigningAlgorithms ?? new List<string>()),
                      model.ShowInDiscoveryDocument, model.Enabled)
                   ==
                   (apiResource.Name, apiResource.DisplayName, apiResource.Description, apiResource.AllowedAccessTokenSigningAlgorithms,
                      apiResource.ShowInDiscoveryDocument, apiResource.Enabled);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestToModel4()
    {
        var client = new Client(ClientTypes.Web, "web-develop", "web-develop");
        var model = client.ToModel();
        var result = (model.ClientId, model.ClientName, model.Description) == (client.ClientId, client.ClientName, client.Description);
        Assert.IsTrue(result);
    }
}
