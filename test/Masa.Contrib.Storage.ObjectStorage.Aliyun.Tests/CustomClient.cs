namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

public class CustomClient : Client
{
    public readonly TemporaryCredentialsResponse TemporaryCredentials;

    public CustomClient(ALiYunStorageOptions options, IMemoryCache cache, ILogger<Client>? logger) : base(options, cache, logger)
    {
        TemporaryCredentials = new(
            "accessKeyId",
            "secretAccessKey",
            "sessionToken",
            DateTime.UtcNow.AddHours(-1));
    }

    protected override TemporaryCredentialsResponse GetTemporaryCredentials(
        string regionId,
        string accessKey,
        string secretKey,
        string roleArn,
        string roleSessionName,
        string policy,
        long durationSeconds)
        => TemporaryCredentials;

    public TemporaryCredentialsResponse TestGetTemporaryCredentials(string regionId,
        string accessKey,
        string secretKey,
        string roleArn,
        string roleSessionName,
        string policy,
        long durationSeconds)
        => base.GetTemporaryCredentials(regionId, accessKey, secretKey, roleArn, roleSessionName, policy, durationSeconds);

    public void TestExpirationTimeLessThan10Second(int durationSeconds)
    {
        var expirationTime = DateTime.UtcNow.AddSeconds(-durationSeconds);
        base.SetTemporaryCredentials(new TemporaryCredentialsResponse("accessKeyId", "secretAccessKey", "sessionToken", expirationTime));
    }
}
