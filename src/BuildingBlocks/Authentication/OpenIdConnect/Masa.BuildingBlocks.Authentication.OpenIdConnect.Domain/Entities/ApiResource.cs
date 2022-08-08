// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;

public class ApiResource : FullAggregateRoot<int, Guid>
{
    private List<ApiResourceSecret> _secrets = new();
    private List<ApiResourceScope> _apiScopes = new();
    private List<ApiResourceClaim> _userClaims = new();
    private List<ApiResourceProperty> _properties = new();

    public bool Enabled { get; private set; }

    public string Name { get; private set; } = "";

    public string DisplayName { get; private set; } = "";

    public string Description { get; private set; } = "";

    public string AllowedAccessTokenSigningAlgorithms { get; private set; } = "";

    public bool ShowInDiscoveryDocument { get; private set; } = true;

    public DateTime? LastAccessed { get; private set; }

    public bool NonEditable { get; private set; }

    public IReadOnlyCollection<ApiResourceSecret> Secrets => _secrets;

    public IReadOnlyCollection<ApiResourceScope> ApiScopes => _apiScopes;

    public IReadOnlyCollection<ApiResourceClaim> UserClaims => _userClaims;

    public IReadOnlyCollection<ApiResourceProperty> Properties => _properties;

    public ApiResource(string name, string displayName, string description, string allowedAccessTokenSigningAlgorithms, bool showInDiscoveryDocument, DateTime? lastAccessed, bool nonEditable, bool enabled)
    {
        Enabled = enabled;
        Name = name;
        DisplayName = displayName;
        Description = description;
        AllowedAccessTokenSigningAlgorithms = allowedAccessTokenSigningAlgorithms;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        LastAccessed = lastAccessed;
        NonEditable = nonEditable;
    }

    public void Update(string displayName, string description, string allowedAccessTokenSigningAlgorithms, bool showInDiscoveryDocument, DateTime? lastAccessed, bool nonEditable, bool enabled)
    {
        Enabled = enabled;
        DisplayName = displayName;
        Description = description;
        AllowedAccessTokenSigningAlgorithms = allowedAccessTokenSigningAlgorithms;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        LastAccessed = lastAccessed;
        NonEditable = nonEditable;
    }

    public void BindUserClaims(List<int> userClaims)
    {
        _userClaims = _userClaims.MergeBy(
           userClaims.Select(userClaim => new ApiResourceClaim(userClaim)),
           item => item.UserClaimId);
    }

    public void BindApiScopes(List<int> apiScopes)
    {
        _apiScopes = _apiScopes.MergeBy(
           apiScopes.Select(apiScope => new ApiResourceScope(apiScope)),
           item => item.ApiScopeId);
    }

    public void BindProperties(Dictionary<string, string> properties)
    {
        _properties.Clear();
        _properties.AddRange(properties.Select(property => new ApiResourceProperty(property.Key, property.Value)));
    }
}

