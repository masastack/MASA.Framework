// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth.Service;

public class LoginService : ILoginService
{
    readonly IHttpClientFactory _httpClientFactory;

    public LoginService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<TokenModel> LoginByPasswordAsync(LoginByPasswordModel login)
    {
        var httpClient = CreateHttpClient();
        var request = new PasswordTokenRequest()
        {
            ClientId = login.ClientId,
            ClientSecret = login.ClientSecret,
            GrantType = GrantType.RESOURCE_OWNER_PASSWORD,
            UserName = login.Account,
            Password = login.Password,
            Scope = string.Join(' ', login.Scope),
        };
        var tokenResponse = await httpClient.RequestPasswordTokenAsync(request);

        return ToModel(tokenResponse);
    }

    public async Task<TokenModel> LoginByPhoneNumberAsync(LoginByPhoneNumberFromSsoModel login)
    {
        var client = CreateHttpClient();

        var paramter = new Dictionary<string, string>
        {
            ["client_Id"] = login.ClientId,
            ["client_secret"] = login.ClientSecret,
            ["grant_type"] = GrantType.PHONE_CODE,
            ["scope"] = string.Join(' ', login.Scope),
            ["PhoneNumber"] = login.PhoneNumber,
            ["code"] = login.Code
        };

        var tokenResponse = await RequestTokenRawAsync(client, paramter);

        return ToModel(tokenResponse);
    }

    public async Task<LoginByThirdPartyIdpResultModel> LoginByThirdPartyIdpAsync(LoginByThirdPartyIdpModel login)
    {
        var client = CreateHttpClient();

        var paramter = new Dictionary<string, string>
        {
            ["client_Id"] = login.ClientId,
            ["client_secret"] = login.ClientSecret,
            ["grant_type"] = GrantType.THIRD_PARTY_IDP,
            ["scope"] = string.Join(' ', login.Scope),
            ["scheme"] = login.Scheme,
            ["code"] = login.Code ?? "",
            ["idToken"] = login.IdToken ?? "",
        };

        var tokenResponse = await RequestTokenRawAsync(client, paramter);
        var tokenModel = ToModel(tokenResponse);
        var thirdPartyUser = tokenResponse.Json.GetProperty("thirdPartyUserData").Deserialize<ThirdPartyIdentityModel>();
        var registerSuccess = tokenResponse.Json.TryGetBoolean("registerSuccess");

        return new LoginByThirdPartyIdpResultModel
        {
            ThirdPartyIdpLoginResultType = registerSuccess is true ? ThirdPartyIdpLoginResultTypes.Success : ThirdPartyIdpLoginResultTypes.NotRegister,
            Token = tokenModel,
            ThirdPartyIdentity = thirdPartyUser
        };
    }

    public async Task<TokenModel> LoginByLdapAsync(LoginByLdapModel login)
    {
        var client = CreateHttpClient();

        var paramter = new Dictionary<string, string>
        {
            ["client_Id"] = login.ClientId,
            ["client_secret"] = login.ClientSecret,
            ["grant_type"] = GrantType.LDAP,
            ["scope"] = string.Join(' ', login.Scope),
            ["userName"] = login.UserName,
            ["scheme"] = login.Scheme
        };

        var tokenResponse = await RequestTokenRawAsync(client, paramter);

        return ToModel(tokenResponse);
    }

    HttpClient CreateHttpClient()
    {
        return _httpClientFactory.CreateClient(DEFAULT_SSO_CLIENT_NAME);
    }

    static async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(HttpClient httpClient)
    {
        var disco = await httpClient.GetDiscoveryDocumentAsync();
        if (disco.IsError)
            throw new UserFriendlyException(disco.Error);

        return disco;
    }

    static async Task<TokenResponse> RequestTokenRawAsync(HttpClient httpClient, Dictionary<string, string> paramter)
    {
        var disco = await GetDiscoveryDocumentAsync(httpClient);
        var tokenResponse = await httpClient.RequestTokenRawAsync(disco.TokenEndpoint, new Parameters(paramter));

        return tokenResponse;
    }

    static TokenModel ToModel(TokenResponse tokenResponse)
    {
        if (tokenResponse.IsError)
            throw new UserFriendlyException(tokenResponse.Error);

        return new TokenModel
        {
            AccessToken = tokenResponse.AccessToken,
            IdentityToken = tokenResponse.IdentityToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiresIn = tokenResponse.ExpiresIn,
        };
    }
}
