// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Constans;

public static class GrantType
{
    [Description("Implicit")]
    public const string IMPLICIT = "implicit";

    [Description("hybrid")]
    public const string HYBRID = "hybrid";

    [Description("AuthorizationCode")]
    public const string AUTHORIZATION_CODE = "authorization_code";

    [Description("ClientCredentials")]
    public const string CLIENT_CREDENTIALS = "client_credentials";

    [Description("ResourceOwnerPassword")]
    public const string RESOURCE_OWNER_PASSWORD = "password";

    [Description("DeviceFlow")]
    public const string DEVICE_FLOW = "urn:ietf:params:oauth:grant-type:device_code";

    [Description("PhoneCode")]
    public const string PHONE_CODE = "phone_code";

    [Description("Phone")]
    public const string LOCAL_PHONE = "local_phone";

    [Description("ThirdPartyIdp")]
    public const string THIRD_PARTY_IDP = "third_party_idp";

    [Description("Ldap")]
    public const string LDAP = "ldap";

    [Description("Impersonation")]
    public const string IMPERSONATION = "impersonation";

    [Description("PssoPhoneCode")]
    public const string PSSO_PHONE_CODE = "psso_phone_code";

    private static readonly List<(string, string)> _disallowCombinations = new List<(string, string)>
    {
        (IMPLICIT, AUTHORIZATION_CODE),
        (IMPLICIT, HYBRID),
        (AUTHORIZATION_CODE, HYBRID),
    };

    public static IReadOnlyCollection<(string, string)> DisallowGrantTypeCombinations => _disallowCombinations.AsReadOnly();
}
