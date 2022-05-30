// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public class DefaultCredentialProvider : ICredentialProvider
{
    private readonly IOssClientFactory _ossClientFactory;
    protected AliyunStorageOptions _options;
    private readonly IMemoryCache _cache;
    protected readonly ILogger<DefaultCredentialProvider>? _logger;

    public bool IncompleteStsOptions { get; private set; }

    public DefaultCredentialProvider(
        IOssClientFactory ossClientFactory,
        AliyunStorageOptions options,
        IMemoryCache cache,
        ILogger<DefaultCredentialProvider>? logger)
    {
        _ossClientFactory = ossClientFactory;
        _options = options;
        IncompleteStsOptions = string.IsNullOrEmpty(options.Sts.RegionId) ||
            string.IsNullOrEmpty(options.RoleArn) &&
            string.IsNullOrEmpty(options.RoleSessionName);
        _cache = cache;
        _logger = logger;
    }

    public DefaultCredentialProvider(
        IOssClientFactory ossClientFactory,
        IOptionsMonitor<AliyunStorageOptions> options,
        IMemoryCache cache,
        ILogger<DefaultCredentialProvider>? logger)
        : this(ossClientFactory, options.CurrentValue, cache, logger)
    {
    }

    public DefaultCredentialProvider(
        IOssClientFactory ossClientFactory,
        IOptionsMonitor<AliyunStorageConfigureOptions> options,
        IMemoryCache cache,
        ILogger<DefaultCredentialProvider>? logger)
        : this(ossClientFactory, GetAliyunStorageOptions(options.CurrentValue), cache, logger)
    {
        options.OnChange(aliyunStorageConfigureOptions =>
        {
            _options = GetAliyunStorageOptions(aliyunStorageConfigureOptions);
            IncompleteStsOptions = !string.IsNullOrEmpty(_options.Sts.RegionId) &&
                !string.IsNullOrEmpty(_options.RoleArn) &&
                !string.IsNullOrEmpty(_options.RoleSessionName);
        });
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
                _options.Sts.RegionId!,
                _options.AccessKeyId,
                _options.AccessKeySecret,
                _options.RoleArn,
                _options.RoleSessionName,
                _options.Policy,
                _options.Sts.GetDurationSeconds());
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

    public virtual void SetTemporaryCredentials(TemporaryCredentialsResponse credentials)
    {
        var timespan = (DateTime.UtcNow - credentials.Expiration!.Value).TotalSeconds - _options.Sts.GetEarlyExpires();
        if (timespan >= 0) _cache.Set(_options.TemporaryCredentialsCacheKey, credentials, TimeSpan.FromSeconds(timespan));
    }

    private static AliyunStorageOptions GetAliyunStorageOptions(AliyunStorageConfigureOptions options)
    {
        AliyunStorageOptions aliyunStorageOptions = options.Storage;
        aliyunStorageOptions.AccessKeyId = TryUpdate(options.Storage.AccessKeyId, options.AccessKeyId)!;
        aliyunStorageOptions.AccessKeySecret = TryUpdate(options.Storage.AccessKeySecret, options.AccessKeySecret)!;
        aliyunStorageOptions.Sts.RegionId = TryUpdate(options.Storage.Sts.RegionId, options.Sts.RegionId);
        aliyunStorageOptions.Sts.DurationSeconds =
            options.Storage.Sts.DurationSeconds ?? options.Sts.DurationSeconds ?? options.Sts.GetDurationSeconds();
        aliyunStorageOptions.Sts.EarlyExpires =
            options.Storage.Sts.EarlyExpires ?? options.Sts.EarlyExpires ?? options.Sts.GetEarlyExpires();
        return aliyunStorageOptions;
    }

    private static string? TryUpdate(string? source, string? destination)
    {
        if (!string.IsNullOrWhiteSpace(source))
            return source;

        return destination;
    }
}
