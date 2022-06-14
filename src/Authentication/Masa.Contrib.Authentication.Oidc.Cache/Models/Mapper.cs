// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.Cache.Models;

public static class Mapper
{
    public static ApiScopeModel ToModel(this ApiScope apiScope)
    {
        return new ApiScopeModel(apiScope.Name, apiScope.DisplayName, apiScope.UserClaims.Select(uc => uc.UserClaim.Name).ToList())
        {
            Required = apiScope.Required,
            Emphasize = apiScope.Emphasize,
            Enabled = apiScope.Enabled,
            Description = apiScope.Description,
            Properties = apiScope.Properties.ToDictionary(p => p.Key, p => p.Value),
            ShowInDiscoveryDocument = apiScope.ShowInDiscoveryDocument,
        };
    }

    public static IdentityResourceModel ToModel(this IdentityResource identityResource)
    {
        return new IdentityResourceModel(identityResource.Name, identityResource.DisplayName, identityResource.UserClaims.Select(uc => uc.UserClaim.Name).ToList())
        {
            Required = identityResource.Required,
            Emphasize = identityResource.Emphasize,
            Enabled = identityResource.Enabled,
            Description = identityResource.Description,
            Properties = identityResource.Properties.ToDictionary(p => p.Key, p => p.Value),
            ShowInDiscoveryDocument = identityResource.ShowInDiscoveryDocument,
        };
    }

    public static ApiResourceModel ToModel(this ApiResource apiResource)
    {
        return new ApiResourceModel(apiResource.Name, apiResource.DisplayName, apiResource.UserClaims.Select(uc => uc.UserClaim.Name).ToList())
        {
            Scopes = apiResource.ApiScopes.Select(a => a.ApiScope.Name).ToList(),
            ApiSecrets = apiResource.Secrets.Select(s => new SecretModel(s.Value, s.Description, s.Expiration)).ToList(),
            AllowedAccessTokenSigningAlgorithms = Convert(apiResource.AllowedAccessTokenSigningAlgorithms),
            Enabled = apiResource.Enabled,
            Description = apiResource.Description,
            ShowInDiscoveryDocument = apiResource.ShowInDiscoveryDocument,
        };

        ICollection<string> Convert(string sourceMember)
        {
            var list = new HashSet<string>();
            if (!string.IsNullOrWhiteSpace(sourceMember))
            {
                sourceMember = sourceMember.Trim();
                foreach (var item in sourceMember.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct())
                {
                    list.Add(item);
                }
            }
            return list;
        }
    }

    public static ClientModel ToModel(this Client client)
    {
        return new ClientModel(client.ClientId.ToString(), client.ClientName, client.Description, client.ClientUri, client.LogoUri, client.RedirectUris.Select(r => r.RedirectUri), client.PostLogoutRedirectUris.Select(p => p.PostLogoutRedirectUri), client.AllowedGrantTypes.Select(a => a.GrantType), client.AllowedScopes.Select(a => a.Scope))
        {
            Enabled = client.Enabled,
            ProtocolType = client.ProtocolType,
            RequireConsent = client.RequireConsent,
            AllowRememberConsent = client.AllowRememberConsent,
            AlwaysIncludeUserClaimsInIdToken = client.AlwaysIncludeUserClaimsInIdToken,
            AllowPlainTextPkce = client.AllowPlainTextPkce,
            RequireRequestObject = client.RequireRequestObject,
            RequirePkce = client.RequirePkce,
            FrontChannelLogoutUri = client.FrontChannelLogoutUri,
            FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired,
            BackChannelLogoutUri = client.BackChannelLogoutUri,
            BackChannelLogoutSessionRequired = client.BackChannelLogoutSessionRequired,
            AllowOfflineAccess = client.AllowOfflineAccess,
            IdentityTokenLifetime = client.IdentityTokenLifetime,
            AccessTokenLifetime = client.AccessTokenLifetime,
            AuthorizationCodeLifetime = client.AuthorizationCodeLifetime,
            ConsentLifetime = client.ConsentLifetime,
            AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime,
            SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime,
            UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh,
            EnableLocalLogin = client.EnableLocalLogin,
            AlwaysSendClientClaims = client.AlwaysSendClientClaims,
            Claims = client.Claims.Select(c => new ClientClaimModel(c.Type, c.Value)).ToList(),
            AllowedCorsOrigins = client.AllowedCorsOrigins.Select(a => a.Origin).ToList(),
            Properties = client.Properties.ToDictionary(p => p.Key, p => p.Value),
            IdentityProviderRestrictions = client.IdentityProviderRestrictions.Select(i => i.Provider).ToList(),
            AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser,
            IncludeJwtId = client.IncludeJwtId,
            AccessTokenType = (AccessTokenType)client.AccessTokenType,
            DeviceCodeLifetime = client.DeviceCodeLifetime,
            UserSsoLifetime = client.UserSsoLifetime,
            RefreshTokenExpiration = (TokenExpiration)client.RefreshTokenExpiration,
            RefreshTokenUsage = (TokenUsage)client.RefreshTokenUsage,
            ClientClaimsPrefix = client.ClientClaimsPrefix,
            PairWiseSubjectSalt = client.PairWiseSubjectSalt,
            UserCodeType = client.UserCodeType,
            ClientSecrets = client.ClientSecrets.Select(s => new SecretModel(s.Type, s.Value)).ToList(),
            AllowedIdentityTokenSigningAlgorithms = client.AllowedIdentityTokenSigningAlgorithms.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList(),
        };
    }
}
