namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

public class CustomNullClient : Client
{
    public string Message = "You are not authorized to do this action. You should be authorized by RAM.";

    public CustomNullClient(ALiYunStorageOptions options, IMemoryCache cache, ILogger<Client>? logger) : base(options, cache, logger)
    {
    }

    protected override TemporaryCredentialsResponse GetTemporaryCredentials(
        string regionId,
        string accessKey,
        string accessSecret,
        string roleArn,
        string roleSessionName,
        string policy,
        long durationSeconds) => throw new Exception(Message);
}
