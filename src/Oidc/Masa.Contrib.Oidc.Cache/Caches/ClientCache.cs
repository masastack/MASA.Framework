// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Oidc.Cache.Caches;

public class ClientCache : IClientCache
{
    IMemoryCacheClient _memoryCacheClient;

    public ClientCache(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<ClientModel?> GetAsync(string clientId)
    {
        string key = $"{CacheKeyConstants.CLIENT_KEY}_{clientId}";
        return await _memoryCacheClient.GetAsync<ClientModel>(key);
    }

    public async Task<List<ClientModel>> GetListAsync()
    {
        var clients = await _memoryCacheClient.GetAsync<List<ClientModel>>(CacheKeyConstants.CLIENT_KEY) ?? new();
        return clients;
    }

    public async Task AddOrUpdateAsync(Client client)
    {
        string key = $"{CacheKeyConstants.CLIENT_KEY}_{client.ClientId}";
        await _memoryCacheClient.SetAsync(key, new ClientModel(client.ClientId.ToString(), client.ClientName, client.Description, client.ClientUri, client.LogoUri, client.RedirectUris.Select(r => r.RedirectUri), client.PostLogoutRedirectUris.Select(p => p.PostLogoutRedirectUri), client.AllowedGrantTypes.Select(a => a.GrantType), client.AllowedScopes.Select(a => a.Scope))
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
            AllowedIdentityTokenSigningAlgorithms = client.AllowedIdentityTokenSigningAlgorithms.Split(",").ToList(),
        });
    }

    public async Task RemoveAsync(Client client)
    {
        string key = $"{CacheKeyConstants.CLIENT_KEY}_{client.ClientId}";
        await _memoryCacheClient.RemoveAsync<ClientModel>(key);
    }
}
