// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public interface IClient
{
    /// <summary>
    /// Obtain temporary authorization credentials through STS service
    /// </summary>
    /// <returns></returns>
    TemporaryCredentialsResponse GetSecurityToken();

    /// <summary>
    /// Obtain temporary request token through authorization service
    /// </summary>
    /// <returns></returns>
    string GetToken();

    Task GetObjectAsync(
        string bucketName,
        string objectName,
        Action<Stream> callback,
        CancellationToken cancellationToken = default(CancellationToken));

    Task GetObjectAsync(
        string bucketName,
        string objectName,
        long offset,
        long length,
        Action<Stream> callback,
        CancellationToken cancellationToken = default(CancellationToken));

    Task PutObjectAsync(
        string bucketName,
        string objectName,
        Stream data,
        CancellationToken cancellationToken = default(CancellationToken));

    Task<bool> ObjectExistsAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default(CancellationToken));

    Task DeleteObjectAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default(CancellationToken));

    Task DeleteObjectAsync(
        string bucketName,
        IEnumerable<string> objectNames,
        CancellationToken cancellationToken = default(CancellationToken));
}
