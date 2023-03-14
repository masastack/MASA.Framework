// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public abstract class AbstractStorageClient :
    IObjectStorageClient
{
    public abstract TemporaryCredentialsResponse GetSecurityToken();

    public abstract string GetToken();

    public abstract Task GetObjectAsync(
        string bucketName,
        string objectName,
        Action<Stream> callback,
        CancellationToken cancellationToken = default);

    public abstract Task GetObjectAsync(
        string bucketName,
        string objectName,
        long offset,
        long length,
        Action<Stream> callback,
        CancellationToken cancellationToken = default);

    public abstract Task PutObjectAsync(
        string bucketName,
        string objectName,
        Stream data,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> ObjectExistsAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default);

    public abstract Task DeleteObjectAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default);

    public abstract Task DeleteObjectAsync(
        string bucketName,
        IEnumerable<string> objectNames,
        CancellationToken cancellationToken = default);
}
