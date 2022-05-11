// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public abstract class BaseClient
{
    protected readonly AliyunStorageOptions _options;
    protected readonly bool _supportSts;
    private readonly IMemoryCache _cache;
    protected readonly ILogger<Client>? _logger;

    public BaseClient(AliyunStorageOptions options, IMemoryCache cache, ILogger<Client>? logger)
    {
        _options = options;
        _supportSts = !string.IsNullOrEmpty(options.RoleArn) && !string.IsNullOrEmpty(options.RoleSessionName);
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Obtain temporary authorization credentials through STS service
    /// </summary>
    /// <returns></returns>
    public virtual TemporaryCredentialsResponse GetSecurityToken()
    {
        if (!_cache.TryGetValue(_options.TemporaryCredentialsCacheKey, out TemporaryCredentialsResponse? temporaryCredentials))
        {
            temporaryCredentials = GetTemporaryCredentials(
                _options.RegionId,
                _options.AccessKeyId,
                _options.AccessKeySecret,
                _options.RoleArn,
                _options.RoleSessionName,
                _options.Policy,
                _options.DurationSeconds);
            SetTemporaryCredentials(temporaryCredentials);
        }
        return temporaryCredentials!;
    }

    protected virtual TemporaryCredentialsResponse GetTemporaryCredentials(
        string regionId,
        string accessKeyId,
        string accessKeySecret,
        string roleArn,
        string roleSessionName,
        string policy,
        long durationSeconds)
    {
        IClientProfile profile = DefaultProfile.GetProfile(regionId, accessKeyId, accessKeySecret);
        DefaultAcsClient client = new DefaultAcsClient(profile);
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
        if (response.HttpResponse.isSuccess())
        {
            return new TemporaryCredentialsResponse(
                response.Credentials.AccessKeyId,
                response.Credentials.AccessKeySecret,
                response.Credentials.SecurityToken,
                DateTime.Parse(response.Credentials.Expiration));
        }

        string responseContent = System.Text.Encoding.Default.GetString(response.HttpResponse.Content);
        string message =
            $"Aliyun.Client: Failed to obtain temporary credentials, RequestId: {response.RequestId}, Status: {response.HttpResponse.Status}, Message: {responseContent}";
        _logger?.LogWarning(
            "Aliyun.Client: Failed to obtain temporary credentials, RequestId: {RequestId}, Status: {Status}, Message: {Message}",
            response.RequestId, response.HttpResponse.Status, responseContent);

        throw new Exception(message);
    }

    protected virtual IOss GetClient()
    {
        var credential = GetCredential();
        return new OssClient(_options.Endpoint, credential.AccessKeyId, credential.AccessKeySecret, credential.SecurityToken);
    }

    protected virtual (string AccessKeyId, string AccessKeySecret, string? SecurityToken) GetCredential()
    {
        if (!_supportSts)
            return new(_options.AccessKeyId, _options.AccessKeySecret, null);

        var securityToken = GetSecurityToken();
        return new(securityToken.AccessKeyId, securityToken.AccessKeySecret, securityToken.SessionToken);
    }

    protected virtual void SetTemporaryCredentials(TemporaryCredentialsResponse credentials)
    {
        var timespan = (DateTime.UtcNow - credentials.Expiration!.Value).TotalSeconds - _options.EarlyExpires;
        if (timespan >= 0)
            _cache.Set(_options.TemporaryCredentialsCacheKey, credentials, TimeSpan.FromSeconds(timespan));
    }
}
