// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Models;

public class ClientModel
{
    private IEnumerable<string>? _allowedGrantTypes;
    private IEnumerable<string>? _apiScopes;

    public string ClientId { get; set; }

    public string ClientName { get; set; }

    public string Description { get; set; }

    public string ClientUri { get; set; }

    public string LogoUri { get; set; }

    public IEnumerable<SecretModel> ClientSecrets { get; set; }

    public IEnumerable<string> RedirectUris { get; set; }

    public IEnumerable<string> PostLogoutRedirectUris { get; set; }

    public IEnumerable<string> AllowedGrantTypes
    {
        get
        {
            if (_allowedGrantTypes is null) throw new Exception("Please set Client.GrantTypes");
            ValidateGrantTypes(_allowedGrantTypes);
            return _allowedGrantTypes.Distinct();
        }
        set
        {
            ValidateGrantTypes(value);
            _allowedGrantTypes = value;
        }
    }

    public IEnumerable<string> AllowedScopes
    {
        get => _apiScopes?.Distinct() ?? throw new Exception("Please set Client.AllowedScopes");
        set => _apiScopes = value;
    }

    /// <summary>
    /// Signing algorithm for identity token. If empty, will use the server default signing
    /// </summary>
    public IEnumerable<string>? AllowedIdentityTokenSigningAlgorithms { get; set; }

    /// <summary>
    /// Allows settings claims for the client (will be included in the access token)
    /// </summary>
    public ICollection<ClientClaimModel>? Claims { get; set; }

    /// <summary>
    /// Gets or sets the allowed CORS origins for JavaScript clients.
    /// </summary>
    public ICollection<string>? AllowedCorsOrigins { get; set; }

    /// <summary>
    ///  Gets or sets the custom properties for the client.
    /// </summary>
    public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Specifies which external IdPs can be used with this client (if list is empty
    ///  all IdPs are allowed). Defaults to empty.
    /// </summary>
    public ICollection<string> IdentityProviderRestrictions { get; set; } = new HashSet<string>();

    public bool RequireConsent { get; set; }

    public bool RequireClientSecret { get; set; } = true;

    public bool Enabled { get; set; } = true;

    public bool AllowRememberConsent { get; set; } = true;

    public bool RequirePkce { get; set; } = true;

    public bool AllowPlainTextPkce { get; set; }

    public bool RequireRequestObject { get; set; }

    public bool AllowAccessTokensViaBrowser { get; set; }

    public bool AllowOfflineAccess { get; set; }

    public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

    public bool BackChannelLogoutSessionRequired { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether JWT access tokens should include an identifier.
    /// </summary>
    public bool IncludeJwtId { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether client claims should be always included
    /// in the access tokens - or only for client credentials flow. Defaults to false
    /// </summary>
    public bool AlwaysSendClientClaims { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the local login is allowed for this client.Defaults to true.
    /// </summary>
    public bool EnableLocalLogin { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the access token (and its claims) should
    /// be updated on a refresh token request. Defaults to false.
    /// </summary>
    public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

    /// <summary>
    /// Lifetime of identity token in seconds (defaults to 300 seconds / 5 minutes)
    /// </summary>
    public int IdentityTokenLifetime { get; set; } = 300;

    /// <summary>
    /// Lifetime of access token in seconds (defaults to 3600 seconds / 1 hour)
    /// </summary>
    public int AccessTokenLifetime { get; set; } = 3600;

    /// <summary>
    /// Lifetime of authorization code in seconds (defaults to 300 seconds / 5 minutes)
    /// </summary>
    public int AuthorizationCodeLifetime { get; set; } = 300;

    /// <summary>
    ///  Maximum lifetime of a refresh token in seconds (defaults to 2592000 seconds / 30 days)
    /// </summary>
    public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;

    /// <summary>
    /// Sliding lifetime of a refresh token in seconds (defaults to 1296000 seconds / 15 days)
    /// </summary>
    public int SlidingRefreshTokenLifetime { get; set; } = 1296000;

    /// <summary>
    /// Lifetime of a user consent in seconds. Defaults to null (no expiration)
    /// </summary>
    public int? ConsentLifetime { get; set; }

    /// <summary>
    /// The maximum duration (in seconds) since the last time the user authenticated.
    /// </summary>
    public int? UserSsoLifetime { get; set; }

    /// <summary>
    /// Gets or sets the device code lifetime.
    /// </summary>
    public int DeviceCodeLifetime { get; set; } = 300;

    /// <summary>
    /// Absolute: the refresh token will expire on a fixed point in time (specified by
    /// the AbsoluteRefreshTokenLifetime) Sliding: when refreshing the token, the lifetime
    /// of the refresh token will be renewed(by the amount specified in SlidingRefreshTokenLifetime).
    /// he lifetime will not exceed AbsoluteRefreshTokenLifetime.
    /// </summary>
    public TokenExpiration RefreshTokenExpiration { get; set; } = TokenExpiration.Absolute;

    /// <summary>
    /// <para>ReUse：the refresh token handle will stay the same when refreshing tokens</para>
    /// <para>OneTimeOnly：the refresh token handle will be updated when refreshing tokens</para>
    /// <para>Default value OneTimeOnly</para>
    /// </summary>
    public TokenUsage RefreshTokenUsage { get; set; } = TokenUsage.OneTimeOnly;

    public AccessTokenType AccessTokenType { get; set; }

    public string ProtocolType { get; set; } = "oidc";

    public string? FrontChannelLogoutUri { get; set; }

    public bool FrontChannelLogoutSessionRequired { get; set; } = true;

    public string? BackChannelLogoutUri { get; set; }

    /// <summary>
    ///  Gets or sets a value to prefix it on client claim types. Defaults to client_.
    /// </summary>
    public string ClientClaimsPrefix { get; set; } = "client_";

    /// <summary>
    /// Gets or sets a salt value used in pair-wise subjectId generation for users of
    /// this client.
    /// </summary>
    public string? PairWiseSubjectSalt { get; set; }

    /// <summary>
    /// Gets or sets the type of the device flow user code.
    /// </summary>
    public string? UserCodeType { get; set; }

    public ClientModel(string clientId, string clientName, string description, string clientUri, string logoUri, IEnumerable<string> redirectUris, IEnumerable<string> postLogoutRedirectUris, IEnumerable<string> allowedGrantTypes, IEnumerable<string> allowedScopes)
    {
        ClientId = clientId;
        ClientName = clientName;
        Description = description;
        ClientUri = clientUri;
        LogoUri = logoUri;
        RedirectUris = redirectUris;
        PostLogoutRedirectUris = postLogoutRedirectUris;
        AllowedGrantTypes = allowedGrantTypes;
        AllowedScopes = allowedScopes;
    }

    private static void ValidateGrantTypes(IEnumerable<string> grantTypes)
    {
        if (grantTypes.Any(grantType => grantType.Contains(' ')))
            throw new InvalidOperationException("Grant types cannot contain spaces");

        if (grantTypes.Count() != 1)
        {
            foreach (var (value1, value2) in GrantType.DisallowGrantTypeCombinations)
            {
                if (grantTypes.Contains(value1, StringComparer.Ordinal) && grantTypes.Contains(value2, StringComparer.Ordinal))
                    throw new InvalidOperationException($"Grant types list cannot contain both {value1} and {value2}");
            }
        }
    }
}

