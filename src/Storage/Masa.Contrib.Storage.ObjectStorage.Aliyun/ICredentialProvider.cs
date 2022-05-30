// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

/// <summary>
/// For internal use, structure may change at any time
/// </summary>
public interface ICredentialProvider
{
    bool IncompleteStsOptions { get; }

    TemporaryCredentialsResponse GetSecurityToken();
}
