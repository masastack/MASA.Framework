// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Oidc.Domain.Entities;

public class Client : FullAggregateRoot<int, Guid>
{
    public ClientTypes ClientType { get; private set; }

    public bool Enabled { get; private set; } = true;

    public string ClientId { get; private set; } = string.Empty;

    public string ProtocolType { get; private set; } = "oidc";

    public List<ClientSecret> ClientSecrets { get; private set; } = new();

    public bool RequireClientSecret { get; private set; } = true;

    public string ClientName { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string ClientUri { get; private set; } = string.Empty;

    public string LogoUri { get; private set; } = string.Empty;

    public bool RequireConsent { get; private set; } = false;

    public bool AllowRememberConsent { get; private set; } = true;

    public bool AlwaysIncludeUserClaimsInIdToken { get; private set; }

    public List<ClientGrantType> AllowedGrantTypes { get; private set; } = new();

    public bool RequirePkce { get; private set; } = true;

    public bool AllowPlainTextPkce { get; private set; }

    public bool RequireRequestObject { get; private set; }

    public bool AllowAccessTokensViaBrowser { get; private set; }

    public List<ClientRedirectUri> RedirectUris { get; private set; } = new();

    public List<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; private set; } = new();

    public string FrontChannelLogoutUri { get; private set; } = string.Empty;

    public bool FrontChannelLogoutSessionRequired { get; private set; } = true;

    public string BackChannelLogoutUri { get; private set; } = string.Empty;

    public bool BackChannelLogoutSessionRequired { get; private set; } = true;

    public bool AllowOfflineAccess { get; private set; }

    public List<ClientScope> AllowedScopes { get; private set; } = new();

    public int IdentityTokenLifetime { get; private set; } = 300;

    public string AllowedIdentityTokenSigningAlgorithms { get; private set; } = string.Empty;

    public int AccessTokenLifetime { get; private set; } = 3600;

    public int AuthorizationCodeLifetime { get; private set; } = 300;

    public int? ConsentLifetime { get; private set; } = null;

    public int AbsoluteRefreshTokenLifetime { get; private set; } = 2592000;

    public int SlidingRefreshTokenLifetime { get; private set; } = 1296000;

    public int RefreshTokenUsage { get; private set; } = (int)TokenUsage.OneTimeOnly;

    public bool UpdateAccessTokenClaimsOnRefresh { get; private set; }

    public int RefreshTokenExpiration { get; private set; } = (int)TokenExpiration.Absolute;

    public int AccessTokenType { get; private set; } = 0; // AccessTokenType.Jwt;

    public bool EnableLocalLogin { get; private set; } = true;

    public List<ClientIdPRestriction> IdentityProviderRestrictions { get; private set; } = new();

    public bool IncludeJwtId { get; private set; }

    public List<ClientClaim> Claims { get; private set; } = new();

    public bool AlwaysSendClientClaims { get; private set; }

    public string ClientClaimsPrefix { get; private set; } = "client_";

    public string PairWiseSubjectSalt { get; private set; } = string.Empty;

    public List<ClientCorsOrigin> AllowedCorsOrigins { get; private set; } = new();

    public List<ClientProperty> Properties { get; private set; } = new();

    public DateTime? LastAccessed { get; private set; }

    public int? UserSsoLifetime { get; private set; }

    public string UserCodeType { get; private set; } = string.Empty;

    public int DeviceCodeLifetime { get; private set; } = 300;

    public bool NonEditable { get; private set; }

    private Client()
    {
    }

    public Client(ClientTypes clientType, string clientId, string clientName)
    {
        SetClientType(clientType);
        ClientId = clientId;
        ClientName = clientName;
    }

    public void SetClientType(ClientTypes clientType)
    {
        ClientType = clientType;
        switch (clientType)
        {
            case ClientTypes.Web:
            case ClientTypes.Spa:
            case ClientTypes.Native:
                AllowedGrantTypes = GrantTypeConsts.Code.Select(x => new ClientGrantType(x)).ToList();
                RequirePkce = true;
                RequireClientSecret = false;
                break;
            case ClientTypes.Machine:
                AllowedGrantTypes = GrantTypeConsts.ClientCredentials.Select(x => new ClientGrantType(x)).ToList();
                RequireClientSecret = true;
                break;
            case ClientTypes.Device:
                AllowedGrantTypes = GrantTypeConsts.DeviceFlow.Select(x => new ClientGrantType(x)).ToList();
                RequireClientSecret = false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetRedirectUris(List<string> redirectUris)
    {
        RedirectUris = redirectUris.Select(x => new ClientRedirectUri(x)).ToList();
    }

    public void SetPostLogoutRedirectUris(List<string> postLogoutRedirectUris)
    {
        PostLogoutRedirectUris = postLogoutRedirectUris.Select(x => new ClientPostLogoutRedirectUri(x)).ToList();
    }

    public void SetAllowedScopes(List<string> allowedScopes)
    {
        AllowedScopes = allowedScopes.Select(x => new ClientScope(x)).ToList();
    }
}

