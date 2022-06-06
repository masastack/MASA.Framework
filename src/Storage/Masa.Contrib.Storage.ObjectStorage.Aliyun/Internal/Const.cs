// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Internal;

internal class Const
{
    public const string TEMPORARY_CREDENTIALS_CACHEKEY = "Aliyun.Storage.TemporaryCredentials";

    public const string DEFAULT_SECTION = "Aliyun";

    public const string INTERNAL_ENDPOINT_SUFFIX = "-internal.aliyuncs.com";

    public const string PUBLIC_ENDPOINT_DOMAIN_SUFFIX = ".aliyuncs.com";

    public const string ERROR_ENDPOINT_MESSAGE = "Unrecognized endpoint, failed to get RegionId";

    public const int DEFAULT_DURATION_SECONDS = 3600;

    public const int DEFAULT_EARLY_EXPIRES = 10;
}
