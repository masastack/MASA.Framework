namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public class Client : IClient
{
    private readonly ALiYunStorageOptions _options;
    private readonly IMemoryCache _cache;
    private readonly ILogger<Client>? _logger;

    public Client(ALiYunStorageOptions options, IMemoryCache cache, ILogger<Client>? logger)
    {
        _options = options;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Obtain temporary authorization credentials through STS service
    /// </summary>
    /// <returns></returns>
    public TemporaryCredentialsResponse GetSecurityToken()
    {
        if (!_cache.TryGetValue(_options.TemporaryCredentialsCacheKey, out TemporaryCredentialsResponse? temporaryCredentials))
        {
            temporaryCredentials = GetTemporaryCredentials(
                _options.RegionId,
                _options.AccessKey,
                _options.SecretKey,
                _options.RoleArn,
                _options.RoleSessionName,
                _options.Policy,
                _options.DurationSeconds);
            SetTemporaryCredentials(temporaryCredentials);
        }
        return temporaryCredentials!;
    }

    /// <summary>
    /// Obtain temporary request token through authorization service
    /// Alibaba Cloud Oss does not support obtaining a temporary authorization token
    /// </summary>
    /// <returns></returns>
    public string GetToken() => throw new NotSupportedException("GetToken is not supported, please use GetSecurityToken");

    protected virtual TemporaryCredentialsResponse GetTemporaryCredentials(
        string regionId,
        string accessKey,
        string secretKey,
        string roleArn,
        string roleSessionName,
        string policy,
        long durationSeconds)
    {
        IClientProfile profile = DefaultProfile.GetProfile(regionId, accessKey, secretKey);
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

        string message = $"Aliyun.Client: Failed to obtain temporary credentials, RequestId: {response.RequestId},Status: {response.HttpResponse.Status}, Message: {System.Text.Encoding.Default.GetString(response.HttpResponse.Content)}";
        _logger?.LogWarning(message);

        throw new Exception(message);
    }

    protected virtual void SetTemporaryCredentials(TemporaryCredentialsResponse credentials)
    {
        var timespan = (DateTime.UtcNow - credentials.Expiration!.Value).TotalSeconds - 10;
        if (timespan >= 0)
            _cache.Set(_options.TemporaryCredentialsCacheKey, credentials, TimeSpan.FromSeconds(timespan));
    }
}
