// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage.Response;

public class TemporaryCredentialsResponse
{
    public string AccessKeyId { get; }

    public string AccessKeySecret { get; }

    public string SessionToken { get; }

    public DateTime? Expiration { get; }

    public TemporaryCredentialsResponse(string accessKeyId, string accessKeySecret, string sessionToken, DateTime? expiration)
    {
        AccessKeyId = accessKeyId;
        AccessKeySecret = accessKeySecret;
        SessionToken = sessionToken;
        Expiration = expiration;
    }
}
