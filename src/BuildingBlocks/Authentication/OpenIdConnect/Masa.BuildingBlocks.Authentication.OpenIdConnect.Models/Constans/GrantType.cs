// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Constans;

public static class GrantType
{
    public const string IMPLICIT = "implicit";

    public const string HYBRID = "hybrid";

    public const string AUTHORIZATION_CODE = "authorization_code";

    public const string CLIENT_CREDENTIALS = "client_credentials";

    public const string RESOURCE_OWNER_PASSWORD = "password";

    public const string DEVICE_FLOW = "urn:ietf:params:oauth:grant-type:device_code";

    public const string PHONE_CODE = "phone_code";

    public const string LOCAL_PHONE = "local_phone";
}
