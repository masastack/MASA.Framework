// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

[TestClass]
public class BaseTest
{
    protected AliyunStorageOptions _aLiYunStorageOptions;
    protected const string HANG_ZHOUE_PUBLIC_ENDPOINT = "oss-cn-hangzhou.aliyuncs.com";

    public BaseTest()
    {
        _aLiYunStorageOptions = new AliyunStorageOptions(
            "AccessKeyId",
            "AccessKeySecret",
            HANG_ZHOUE_PUBLIC_ENDPOINT,
            "RoleArn",
            "RoleSessionName");
    }
}
