// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Oidc.Domain.Entities;

public class IdentityResource : FullAggregateRoot<int, Guid>
{
    private List<IdentityResourceClaim> _userClaims = new();
    private List<IdentityResourceProperty> _properties = new();

    public string Name { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public bool Enabled { get; private set; } = true;

    public bool Required { get; private set; }

    public bool Emphasize { get; private set; }

    public bool ShowInDiscoveryDocument { get; private set; } = true;

    public IReadOnlyCollection<IdentityResourceClaim> UserClaims => _userClaims;

    public IReadOnlyCollection<IdentityResourceProperty> Properties => _properties;

    public bool NonEditable { get; private set; }

    public IdentityResource(string name, string displayName, string description, bool enabled, bool required, bool emphasize, bool showInDiscoveryDocument, bool nonEditable)
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
        Enabled = enabled;
        Required = required;
        Emphasize = emphasize;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        NonEditable = nonEditable;
    }

    public void BindUserClaims(IEnumerable<int> userClaims)
    {
        _userClaims.Clear();
        _userClaims.AddRange(userClaims.Select(id => new IdentityResourceClaim(id)));
    }

    public void BindProperties(Dictionary<string, string> properties)
    {
        _properties.Clear();
        _properties.AddRange(properties.Select(property => new IdentityResourceProperty(property.Key, property.Value)));
    }

    public void Update(string displayName, string description, bool enabled, bool required, bool emphasize, bool showInDiscoveryDocument, bool nonEditable)
    {
        DisplayName = displayName;
        Description = description;
        Enabled = enabled;
        Required = required;
        Emphasize = emphasize;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        NonEditable = nonEditable;
    }
}

