// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Oidc.Models.Models;

public class ApiScopeModel : ResourceModel
{
    public bool Required { get; set; }

    public bool Emphasize { get; set; }

    public ApiScopeModel()
    {

    }

    public ApiScopeModel(
        string name,
        string? displayName = null,
        ICollection<string>? userClaims = null)
    {
        Name = name;
        DisplayName = displayName ?? name;
        if (userClaims is not null) UserClaims = userClaims;
    }

    public ApiScopeModel(
        string name,
        string displayName,
        string? description,
        bool enabled,
        bool showInDiscoveryDocument,
        bool required,
        bool emphasize,
        ICollection<string>? userClaims,
        Dictionary<string, string> properties) : this(name, displayName, userClaims)
    {
        Description = description;
        Enabled = enabled;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        Required = required;
        Emphasize = emphasize;
        Properties = properties;
    }
}

