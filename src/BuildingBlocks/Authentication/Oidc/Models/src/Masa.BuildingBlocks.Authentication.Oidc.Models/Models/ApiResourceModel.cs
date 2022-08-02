// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Oidc.Models.Models;

public class ApiResourceModel : ResourceModel
{
    public ICollection<string>? Scopes { get; set; }

    public ICollection<SecretModel>? ApiSecrets { get; set; }

    public ICollection<string>? AllowedAccessTokenSigningAlgorithms { get; set; }

    public ApiResourceModel()
    {

    }

    public ApiResourceModel(
        string name,
        string? displayName = null,
        ICollection<string>? userClaims = null)
    {
        Name = name;
        DisplayName = displayName ?? name;
        if (userClaims is not null) UserClaims = userClaims;
    }

    public ApiResourceModel(
        string name,
        string displayName,
        string? description,
        bool enabled,
        bool showInDiscoveryDocument,
        ICollection<string>? userClaims,
        ICollection<string>? scopes,
        IDictionary<string, string> properties,
        ICollection<SecretModel>? apiSecrets,
        ICollection<string>? allowedAccessTokenSigningAlgorithms) : this(name, displayName, userClaims)
    {
        Description = description;
        Enabled = enabled;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        Scopes = scopes;
        Properties = properties;
        ApiSecrets = apiSecrets;
        AllowedAccessTokenSigningAlgorithms = allowedAccessTokenSigningAlgorithms;
    }
}

