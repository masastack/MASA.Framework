// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Consts;

public static class CacheKeyConsts
{
    public const string ALL_THIRD_PARTY_IDP = "get_all_thirdparty_idp";
    public const string USER_BY_ID = "get_user_by_id";

    public static string UserKey(Guid userId)
    {
        return $"{USER_BY_ID}{userId}";
    }
}
