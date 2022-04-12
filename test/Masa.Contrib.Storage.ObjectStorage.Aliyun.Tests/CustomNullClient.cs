namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

public class CustomNullClient : Client
{
    public string Message = "You are not authorized to do this action. You should be authorized by RAM.";

    public CustomNullClient(ALiYunStorageOptions options, IMemoryCache cache, ILogger<Client>? logger) : base(options, cache, logger)
    {
    }

    protected override TemporaryCredentialsResponse? GetTemporaryCredentials(
        string regionId,
        string accessKey,
        string secretKey,
        string roleArn,
        string roleSessionName,
        string policy,
        long durationSeconds,
        Action<string> error)
    {
        error.Invoke(Message);
        return null;
    }
}
