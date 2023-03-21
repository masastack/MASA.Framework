// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public class DefaultCredentialProvider : ICredentialProvider
{
    private readonly IOssClientFactory _ossClientFactory;
    private readonly IMemoryCache _cache;
    protected readonly ILogger<DefaultCredentialProvider>? _logger = null;

    private AliyunStorageOptions _aliyunStorageOptions;

    public DefaultCredentialProvider(
        AliyunStorageOptions aliyunStorageOptions,
        IMemoryCache cache,
        IOssClientFactory? ossClientFactory = null,
        ILoggerFactory? loggerFactory = null)
    {
        _aliyunStorageOptions = aliyunStorageOptions;
        _cache = cache;
        _ossClientFactory = ossClientFactory ?? new DefaultOssClientFactory();
        _cache = cache;
        _logger = loggerFactory?.CreateLogger<DefaultCredentialProvider>();
    }

    /// <summary>
    /// Obtain temporary authorization credentials through STS service
    /// </summary>
    /// <returns></returns>
    public virtual TemporaryCredentialsResponse GetSecurityToken()
    {
        if (!_cache.TryGetValue(_aliyunStorageOptions.TemporaryCredentialsCacheKey, out TemporaryCredentialsResponse? temporaryCredentials))
        {
            temporaryCredentials = GetTemporaryCredentials(
                _aliyunStorageOptions.Sts!.RegionId!,
                _aliyunStorageOptions.AccessKeyId,
                _aliyunStorageOptions.AccessKeySecret,
                _aliyunStorageOptions.RoleArn,
                _aliyunStorageOptions.RoleSessionName,
                _aliyunStorageOptions.Policy,
                _aliyunStorageOptions.Sts.GetDurationSeconds());
            SetTemporaryCredentials(temporaryCredentials);
        }
        return temporaryCredentials!;
    }

    public virtual TemporaryCredentialsResponse GetTemporaryCredentials(
        string regionId,
        string accessKeyId,
        string accessKeySecret,
        string roleArn,
        string roleSessionName,
        string policy,
        long durationSeconds)
    {
        IAcsClient client = _ossClientFactory.GetAcsClient(accessKeyId, accessKeySecret, regionId);
        var request = new AssumeRoleRequest
        {
            ContentType = AliyunFormatType.JSON,
            RoleArn = roleArn,
            RoleSessionName = roleSessionName,
            DurationSeconds = durationSeconds
        };
        if (!string.IsNullOrEmpty(policy))
            request.Policy = policy;
        var response = client.GetAcsResponse(request);
        // if (response.HttpResponse.isSuccess()) //todo: Get Sts response information is null, waiting for repair: https://github.com/aliyun/aliyun-openapi-net-sdk/pull/401
        // {
        return new TemporaryCredentialsResponse(
            response.Credentials.AccessKeyId,
            response.Credentials.AccessKeySecret,
            response.Credentials.SecurityToken,
            DateTime.Parse(response.Credentials.Expiration));
        // }

        // string responseContent = Encoding.Default.GetString(response.HttpResponse.Content);
        // string message =
        //     $"Aliyun.Client: Failed to obtain temporary credentials, RequestId: {response.RequestId}, Status: {response.HttpResponse.Status}, Message: {responseContent}";
        // _logger?.LogWarning(
        //     "Aliyun.Client: Failed to obtain temporary credentials, RequestId: {RequestId}, Status: {Status}, Message: {Message}",
        //     response.RequestId, response.HttpResponse.Status, responseContent);
        //
        // throw new Exception(message);
    }

    protected virtual void SetTemporaryCredentials(TemporaryCredentialsResponse credentials)
    {
        var timespan = (DateTime.UtcNow - credentials.Expiration!.Value).TotalSeconds - _aliyunStorageOptions.Sts!.GetEarlyExpires();
        if (timespan >= 0) _cache.Set(_aliyunStorageOptions.TemporaryCredentialsCacheKey, credentials, TimeSpan.FromSeconds(timespan));
    }
}
