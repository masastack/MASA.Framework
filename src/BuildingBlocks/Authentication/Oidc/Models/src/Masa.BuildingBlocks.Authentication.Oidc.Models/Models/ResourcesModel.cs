// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Oidc.Models.Models;

public class ResourcesModel
{
    /// <summary>
    /// Gets or sets a value indicating whether [offline access].
    /// </summary>
    public bool OfflineAccess { get; set; }

    public IEnumerable<IdentityResourceModel> IdentityResources { get; set; } = new HashSet<IdentityResourceModel>();

    public IEnumerable<ApiResourceModel> ApiResources { get; set; } = new HashSet<ApiResourceModel>();

    public IEnumerable<ApiScopeModel> ApiScopes { get; set; } = new HashSet<ApiScopeModel>();

    public ResourcesModel()
    {
    }

    public ResourcesModel(ResourcesModel other)
        : this(other.IdentityResources, other.ApiResources, other.ApiScopes)
    {
        OfflineAccess = other.OfflineAccess;
    }

    public ResourcesModel(IEnumerable<IdentityResourceModel> identityResources, IEnumerable<ApiResourceModel> apiResources, IEnumerable<ApiScopeModel> apiScopes)
    {
        if (identityResources?.Any() == true)
        {
            IdentityResources = new HashSet<IdentityResourceModel>(identityResources.ToArray());
        }
        if (apiResources?.Any() == true)
        {
            ApiResources = new HashSet<ApiResourceModel>(apiResources.ToArray());
        }
        if (apiScopes?.Any() == true)
        {
            ApiScopes = new HashSet<ApiScopeModel>(apiScopes.ToArray());
        }
    }
}

