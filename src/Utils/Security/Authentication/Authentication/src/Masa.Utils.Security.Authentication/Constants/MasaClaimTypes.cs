// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Authentication.Constants;

public struct MasaClaimTypes
{
    public const string USER_NAME = ClaimTypes.Name;

    public const string NAME = ClaimTypes.GivenName;

    public const string USER_ID = ClaimTypes.NameIdentifier;

    public const string ROLE = ClaimTypes.Role;

    public const string EMAIL = ClaimTypes.Email;

    public const string PHONE_NUMBER = ClaimTypes.MobilePhone;

    public const string EMAIL_VERIFIED = "https://masastack.com/security/authentication/email_verified";

    public const string PHONE_NUMBER_VERIFIED = "https://masastack.com/security/authentication/phone_number_verified";

    public const string ENVIRONMENT = "https://masastack.com/security/authentication/environment";

    public const string TENANT = "https://masastack.com/security/authentication/tenant";
}
