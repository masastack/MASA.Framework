// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Text.Json;
using Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Models;

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

    [TestMethod]
    public void TestToModelClientSecretsKeepType()
    {
        var client = new Client(ClientTypes.Web, "web-develop", "web-develop");
        client.ClientSecrets.Add(new ClientSecret("shared", "hashed-value", null, "SharedSecret"));
        client.ClientSecrets.Add(new ClientSecret("jwk", "{\"kty\":\"RSA\"}", null, "JWK"));
        client.ClientSecrets.Add(new ClientSecret("cert", "MIIB", null, "X509CertificateBase64"));

        var model = client.ToModel();

        CollectionAssert.AreEqual(
            new[] { "SharedSecret", "JWK", "X509CertificateBase64" },
            model.ClientSecrets.Select(s => s.Type).ToArray());

        // 缓存里存的是序列化后的模型，Type 也要能随 JSON 往返保留
        var roundTripped = JsonSerializer.Deserialize<List<SecretModel>>(JsonSerializer.Serialize(model.ClientSecrets.ToList()))!;
        CollectionAssert.AreEqual(
            new[] { "SharedSecret", "JWK", "X509CertificateBase64" },
            roundTripped.Select(s => s.Type).ToArray());
    }
}
