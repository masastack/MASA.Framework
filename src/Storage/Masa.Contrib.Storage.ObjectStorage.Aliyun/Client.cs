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
    public ResponseBase<TemporaryCredentialsResponse> GetSecurityToken()
    {
        if (!_cache.TryGetValue(_options.TemporaryCredentialsCacheKey, out TemporaryCredentialsResponse? temporaryCredentials))
        {
            var errorMessage = "";
            temporaryCredentials = GetTemporaryCredentials(
                _options.RegionId,
                _options.AccessKey,
                _options.SecretKey,
                _options.RoleArn,
                _options.RoleSessionName,
                _options.Policy,
                _options.DurationSeconds, error => errorMessage = error);
            if (temporaryCredentials != null)
                SetTemporaryCredentials(temporaryCredentials);
            else
                return ResponseBase<TemporaryCredentialsResponse>.Error(errorMessage);
        }
        return ResponseBase<TemporaryCredentialsResponse>.Success(temporaryCredentials!);
    }

    /// <summary>
    /// Obtain temporary request token through authorization service
    /// Alibaba Cloud Oss does not support obtaining a temporary authorization token
    /// </summary>
    /// <returns></returns>
    public ResponseBase<string> GetToken() => ResponseBase<string>.Error("GetToken is not supported, please use GetSecurityToken");

    protected virtual TemporaryCredentialsResponse? GetTemporaryCredentials(
        string regionId,
        string accessKey,
        string secretKey,
        string roleArn,
        string roleSessionName,
        string policy,
        long durationSeconds,
        Action<string> error)
    {
        try
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
                    response.Credentials.Expiration);
            }
            string message = System.Text.Encoding.Default.GetString(response.HttpResponse.Content);
            error.Invoke($"Failed to obtain temporary credentials, RequestId: {response.RequestId},Status: {response.HttpResponse.Status}, Message: {message}");
            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to get TemporaryCredentials");
            error.Invoke(ex.Message);
            return null;
        }
    }

    protected virtual void SetTemporaryCredentials(TemporaryCredentialsResponse credentials)
    {
        DateTime expirationTime = DateTime.SpecifyKind(DateTime.Parse(credentials.Expiration), DateTimeKind.Utc);
        var timespan = (DateTime.UtcNow - expirationTime).TotalSeconds - 10;
        if (timespan >= 0)
            _cache.Set(_options.TemporaryCredentialsCacheKey, credentials, TimeSpan.FromSeconds(timespan));
    }
}
