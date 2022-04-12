namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

public class CustomClient : Client
{
    public TemporaryCredentialsResponse TemporaryCredentials => new(
        "accessKeyId",
        "secretAccessKey",
        "sessionToken",
        DateTime.UtcNow.AddHours(-1).ToString(CultureInfo.InvariantCulture));

    public CustomClient(ALiYunStorageOptions options, IMemoryCache cache, ILogger<Client>? logger) : base(options, cache, logger)
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
        => TemporaryCredentials;

    public TemporaryCredentialsResponse? TestGetTemporaryCredentials(string regionId,
        string accessKey,
        string secretKey,
        string roleArn,
        string roleSessionName,
        string policy,
        long durationSeconds,
        Action<string> error)
    {
        return base.GetTemporaryCredentials(regionId, accessKey, secretKey, roleArn, roleSessionName, policy, durationSeconds, error);
    }

    public void TestExpirationTimeLessThan10Second(int durationSeconds)
    {
        var expirationTime = DateTime.UtcNow.AddSeconds(-durationSeconds).ToString(CultureInfo.InvariantCulture);
        base.SetTemporaryCredentials(new TemporaryCredentialsResponse("accessKeyId", "secretAccessKey", "sessionToken", expirationTime));
    }
}
