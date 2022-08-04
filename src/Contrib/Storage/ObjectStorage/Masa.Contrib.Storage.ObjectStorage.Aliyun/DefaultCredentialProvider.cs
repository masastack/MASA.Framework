// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public class DefaultCredentialProvider : ICredentialProvider
{
    private readonly IOssClientFactory _ossClientFactory;
    private readonly IAliyunStorageOptionProvider _optionProvider;
    private readonly IMemoryCache _cache;
    protected readonly ILogger<DefaultCredentialProvider>? _logger;

    private AliyunStorageOptions Options => _optionProvider.GetOptions();

    public DefaultCredentialProvider(
        IOssClientFactory ossClientFactory,
        IAliyunStorageOptionProvider optionProvider,
        IMemoryCache cache,
        ILogger<DefaultCredentialProvider>? logger = null)
    {
        _ossClientFactory = ossClientFactory;
        _optionProvider = optionProvider;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Obtain temporary authorization credentials through STS service
    /// </summary>
    /// <returns></returns>
    public virtual TemporaryCredentialsResponse GetSecurityToken()
    {
        if (!_cache.TryGetValue(Options.TemporaryCredentialsCacheKey, out TemporaryCredentialsResponse? temporaryCredentials))
        {
            temporaryCredentials = GetTemporaryCredentials(
                Options.Sts.RegionId!,
                Options.AccessKeyId,
                Options.AccessKeySecret,
                Options.RoleArn,
                Options.RoleSessionName,
                Options.Policy,
                Options.Sts.GetDurationSeconds());
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
        var timespan = (DateTime.UtcNow - credentials.Expiration!.Value).TotalSeconds - Options.Sts.GetEarlyExpires();
        if (timespan >= 0) _cache.Set(Options.TemporaryCredentialsCacheKey, credentials, TimeSpan.FromSeconds(timespan));
    }
}
