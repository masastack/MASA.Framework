// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;

public class ApiScope : FullAggregateRoot<Guid, Guid>
{
    private List<ApiScopeClaim> _userClaims = new();
    private List<ApiScopeProperty> _properties = new();

    public bool Enabled { get; private set; }

    public string Name { get; private set; } = "";

    public string DisplayName { get; private set; } = "";

    public string Description { get; private set; } = "";

    public bool Required { get; private set; }

    public bool Emphasize { get; private set; }

    public bool ShowInDiscoveryDocument { get; private set; }

    public IReadOnlyCollection<ApiScopeClaim> UserClaims => _userClaims;

    public IReadOnlyCollection<ApiScopeProperty> Properties => _properties;

    public ApiScope(string name) : this(name, name, "", true, true, true, true)
    {

    }

    public ApiScope(string name, string displayName, string description, bool required, bool emphasize, bool showInDiscoveryDocument, bool enabled)
    {
        Enabled = enabled;
        Name = name;
        DisplayName = displayName;
        Description = description;
        Required = required;
        Emphasize = emphasize;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
    }

    public void Update(string displayName, string description, bool required, bool emphasize, bool showInDiscoveryDocument, bool enabled)
    {
        Enabled = enabled;
        DisplayName = displayName;
        Description = description;
        Required = required;
        Emphasize = emphasize;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
    }

    public void BindUserClaims(List<Guid> userClaims)
    {
        _userClaims = _userClaims.MergeBy(
           userClaims.Select(userClaim => new ApiScopeClaim(userClaim)),
           item => item.UserClaimId);
    }

    public void BindProperties(Dictionary<string, string> properties)
    {
        _properties.Clear();
        _properties.AddRange(properties.Select(property => new ApiScopeProperty(property.Key, property.Value)));
    }
}
