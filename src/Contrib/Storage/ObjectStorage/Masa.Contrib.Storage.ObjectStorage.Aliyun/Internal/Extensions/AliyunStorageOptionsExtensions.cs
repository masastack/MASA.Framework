// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

internal static class AliyunStorageOptionsExtensions
{
    public static bool IsSupportCallback(this AliyunStorageOptions aliyunStorageOptions)
        => !string.IsNullOrEmpty(aliyunStorageOptions.CallbackBody) &&
            !string.IsNullOrEmpty(aliyunStorageOptions.CallbackUrl);

    public static bool IsIncompleteStsOptions(this AliyunStorageOptions aliyunStorageOptions)
        => aliyunStorageOptions.Sts == null ||
            string.IsNullOrEmpty(aliyunStorageOptions.Sts.RegionId) ||
            string.IsNullOrEmpty(aliyunStorageOptions.RoleArn) ||
            string.IsNullOrEmpty(aliyunStorageOptions.RoleSessionName);
}
