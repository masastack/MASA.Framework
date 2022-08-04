// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Models;

public class IdentityResourceModel : ResourceModel
{
    public bool Required { get; set; }

    public bool Emphasize { get; set; }

    public override ICollection<string> UserClaims { get; set; }

    public IdentityResourceModel()
    {
    }

    public IdentityResourceModel(string name, string displayName, ICollection<string> userClaims)
    {
        Name = name;
        DisplayName = displayName;
        UserClaims = userClaims;
    }

    public IdentityResourceModel(string name, ICollection<string> userClaims) : this(name, name, userClaims)
    {
    }

    public IdentityResourceModel(
        string name,
        string displayName,
        string? description,
        bool enabled,
        bool showInDiscoveryDocument,
        bool required,
        bool emphasize,
        ICollection<string> userClaims,
        IDictionary<string, string> properties) : this(name, displayName, userClaims)
    {
        Description = description;
        Enabled = enabled;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        Required = required;
        Emphasize = emphasize;
        Properties = properties;
    }
}

