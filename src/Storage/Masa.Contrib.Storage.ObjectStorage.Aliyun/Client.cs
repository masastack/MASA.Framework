// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public class Client : BaseClient, IClient
{
    private readonly bool _supportCallback;

    public Client(AliyunStorageOptions options, IMemoryCache cache, ILogger<Client>? logger)
        : base(options, cache, logger)
    {
        _supportCallback = !string.IsNullOrEmpty(options.CallbackBody) && !string.IsNullOrEmpty(options.CallbackUrl);
    }

    /// <summary>
    /// Obtain temporary authorization credentials through STS service
    /// </summary>
    /// <returns></returns>
    public override TemporaryCredentialsResponse GetSecurityToken()
    {
        if (!_supportSts)
            throw new ArgumentNullException($"{nameof(_options.RoleArn)} or {nameof(_options.RoleSessionName)} cannot be empty or null");

        return base.GetSecurityToken();
    }

    /// <summary>
    /// Obtain temporary request token through authorization service
    /// Alibaba Cloud Oss does not support obtaining a temporary authorization token
    /// </summary>
    /// <returns></returns>
    public string GetToken() => throw new NotSupportedException("GetToken is not supported, please use GetSecurityToken");

    public Task GetObjectAsync(
        string bucketName,
        string objectName,
        Action<Stream> callback,
        CancellationToken cancellationToken = default)
    {
        var client = GetClient();
        var result = client.GetObject(bucketName, objectName);
        callback.Invoke(result.Content);
        return Task.CompletedTask;
    }

    public Task GetObjectAsync(
        string bucketName,
        string objectName,
        long offset,
        long length,
        Action<Stream> callback,
        CancellationToken cancellationToken = default)
    {
        if (length < 0 && length != -1)
            throw new ArgumentOutOfRangeException(nameof(length), $"{length} should be greater than 0 or -1");

        var client = GetClient();
        var request = new GetObjectRequest(bucketName, objectName);
        request.SetRange(offset, length > 0 ? offset + length : length);
        var result = client.GetObject(request);
        callback.Invoke(result.Content);
        return Task.CompletedTask;
    }

    public Task PutObjectAsync(
        string bucketName,
        string objectName,
        Stream data,
        CancellationToken cancellationToken = default)
    {
        var client = GetClient();
        var objectMetadata = _supportCallback ? BuildCallbackMetadata(_options.CallbackUrl, _options.CallbackBody) : null;
        var result = !_options.EnableResumableUpload || _options.BigObjectContentLength > data.Length ?
            client.PutObject(bucketName, objectName, data, objectMetadata) :
            client.ResumableUploadObject(new UploadObjectRequest(bucketName, objectName, data)
            {
                PartSize = _options.PartSize,
                Metadata = objectMetadata
            });
        _logger?.LogDebug("----- Upload {ObjectName} from {BucketName} - ({Result})",
            objectName,
            bucketName,
            new UploadObjectResponse(result));
        return Task.CompletedTask;
    }

    protected virtual ObjectMetadata BuildCallbackMetadata(string callbackUrl, string callbackBody)
    {
        string callbackHeaderBuilder = new CallbackHeaderBuilder(callbackUrl, callbackBody).Build();
        var metadata = new ObjectMetadata();
        metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);
        return metadata;
    }

    public Task<bool> ObjectExistsAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default)
    {
        var client = GetClient();
        var exist = client.DoesObjectExist(bucketName, objectName);
        return Task.FromResult(exist);
    }

    public async Task DeleteObjectAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default)
    {
        var client = GetClient();
        if (await ObjectExistsAsync(bucketName, objectName, cancellationToken))
        {
            var result = client.DeleteObject(bucketName, objectName);
            _logger?.LogDebug("----- Delete {ObjectName} from {BucketName} - ({Result})",
                objectName,
                bucketName,
                result);
        }
    }

    public Task DeleteObjectAsync(
        string bucketName,
        IEnumerable<string> objectNames,
        CancellationToken cancellationToken = default)
    {
        var client = GetClient();
        var result = client.DeleteObjects(new DeleteObjectsRequest(bucketName, objectNames.ToList(), _options.Quiet));
        _logger?.LogDebug("----- Delete {ObjectNames} from {BucketName} - ({Result})",
            objectNames,
            bucketName,
            result);
        return Task.CompletedTask;
    }
}
