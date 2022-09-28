// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

public abstract class TestBase
{
    protected AliyunStorageOptions _aLiYunStorageOptions;
    protected const string HANG_ZHOUE_PUBLIC_ENDPOINT = "oss-cn-hangzhou.aliyuncs.com";

    protected TestBase()
    {
        _aLiYunStorageOptions = new AliyunStorageOptions(
            "AccessKeyId",
            "AccessKeySecret",
            HANG_ZHOUE_PUBLIC_ENDPOINT,
            "RoleArn",
            "RoleSessionName");
    }

    protected Mock<IAliyunStorageOptionProvider> MockOptionProvider(bool? incompleteStsOptions = null)
    {
        Mock<IAliyunStorageOptionProvider> optionProvider = new();
        if (incompleteStsOptions != null)
            optionProvider.Setup(provider => provider.IncompleteStsOptions).Returns(incompleteStsOptions.Value);
        optionProvider.Setup(provider => provider.GetOptions()).Returns(_aLiYunStorageOptions);
        return optionProvider;
    }
}
