// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public class DefaultObjectStorageClientContainer<TContainer>
    : DefaultObjectStorageClientContainer, IObjectStorageClientContainer<TContainer> where TContainer : class
{
    public DefaultObjectStorageClientContainer(IObjectStorageClient client, IBucketNameProvider bucketNameProvider)
        : base(client, bucketNameProvider.GetBucketName<TContainer>())
    {
    }
}

public class DefaultObjectStorageClientContainer : IObjectStorageClientContainer
{
    private readonly IObjectStorageClient _client;
    private readonly string _bucketName;

    public DefaultObjectStorageClientContainer(IObjectStorageClient client, string bucketName)
    {
        _client = client;
        _bucketName = bucketName;
    }

    public TemporaryCredentialsResponse GetSecurityToken() => _client.GetSecurityToken();

    public string GetToken() => _client.GetToken();

    public Task GetObjectAsync(string objectName, Action<Stream> callback, CancellationToken cancellationToken = default)
        => _client.GetObjectAsync(_bucketName, objectName, callback, cancellationToken);

    public Task GetObjectAsync(string objectName,
        long offset,
        long length,
        Action<Stream> callback,
        CancellationToken cancellationToken = default)
        => _client.GetObjectAsync(_bucketName, objectName, offset, length, callback, cancellationToken);

    public Task PutObjectAsync(string objectName, Stream data, CancellationToken cancellationToken = default)
        => _client.PutObjectAsync(_bucketName, objectName, data, cancellationToken);

    public Task<bool> ObjectExistsAsync(string objectName, CancellationToken cancellationToken = default)
        => _client.ObjectExistsAsync(_bucketName, objectName, cancellationToken);

    public Task DeleteObjectAsync(string objectName, CancellationToken cancellationToken = default)
        => _client.DeleteObjectAsync(_bucketName, objectName, cancellationToken);

    public Task DeleteObjectAsync(IEnumerable<string> objectNames, CancellationToken cancellationToken = default)
        => _client.DeleteObjectAsync(_bucketName, objectNames, cancellationToken);
}
