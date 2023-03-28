// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests.Isolation")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

internal class DefaultObjectStorageClient : IManualObjectStorageClient
{
    public readonly IManualObjectStorageClient _objectStorageClient;

    public DefaultObjectStorageClient(IManualObjectStorageClient objectStorageClient)
        => _objectStorageClient = objectStorageClient;

    public TemporaryCredentialsResponse GetSecurityToken()
        => _objectStorageClient.GetSecurityToken();

    public string GetToken()
        => _objectStorageClient.GetToken();

    public Task GetObjectAsync(
        string bucketName,
        string objectName,
        Action<Stream> callback,
        CancellationToken cancellationToken = default(CancellationToken))
        => _objectStorageClient.GetObjectAsync(bucketName, objectName, callback, cancellationToken);

    public Task GetObjectAsync(
        string bucketName,
        string objectName,
        long offset,
        long length,
        Action<Stream> callback,
        CancellationToken cancellationToken = default(CancellationToken))
        => _objectStorageClient.GetObjectAsync(bucketName, objectName, offset, length, callback, cancellationToken);

    public Task PutObjectAsync(
        string bucketName,
        string objectName,
        Stream data,
        CancellationToken cancellationToken = default(CancellationToken))
        => _objectStorageClient.PutObjectAsync(bucketName, objectName, data, cancellationToken);

    public Task<bool> ObjectExistsAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default(CancellationToken))
        => _objectStorageClient.ObjectExistsAsync(bucketName, objectName, cancellationToken);

    public Task DeleteObjectAsync(string bucketName, string objectName, CancellationToken cancellationToken = default(CancellationToken))
        => _objectStorageClient.DeleteObjectAsync(bucketName, objectName, cancellationToken);

    public Task DeleteObjectAsync(
        string bucketName,
        IEnumerable<string> objectNames,
        CancellationToken cancellationToken = default(CancellationToken))
        => _objectStorageClient.DeleteObjectAsync(bucketName, objectNames, cancellationToken);

    public void Dispose()
    {
        //don't need to be released
    }
}
