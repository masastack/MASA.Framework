// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Options;

public class AliyunStorageOptions
{
    public string AccessKeyId { get; set; }

    public string AccessKeySecret { get; set; }

    public string RegionId { get; set; }

    public string Endpoint { get; set; }

    public string RoleArn { get; set; }

    public string RoleSessionName { get; set; }

    private int _durationSeconds;

    /// <summary>
    /// Set the validity period of the temporary access credential, the minimum is 900, and the maximum is 43200.
    /// default: 3600
    /// unit: second
    /// </summary>
    public int DurationSeconds
    {
        get => _durationSeconds;
        set
        {
            if (value < 900 || value > 43200)
                throw new ArgumentOutOfRangeException(nameof(DurationSeconds), $"{nameof(DurationSeconds)} must be in range of 900-43200");

            _durationSeconds = value;
        }
    }

    /// <summary>
    /// If policy is empty, the user will get all permissions under this role
    /// </summary>
    public string Policy { get; set; }

    private string _temporaryCredentialsCacheKey;

    public string TemporaryCredentialsCacheKey
    {
        get => _temporaryCredentialsCacheKey;
        set => _temporaryCredentialsCacheKey = CheckNullOrEmptyAndReturnValue(value, nameof(TemporaryCredentialsCacheKey));
    }

    private int _earlyExpires = 10;

    /// <summary>
    /// Voucher expires early
    /// default: 10
    /// unit: second
    /// </summary>
    public int EarlyExpires
    {
        get => _earlyExpires;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(EarlyExpires), $"{nameof(EarlyExpires)} must be Greater than 0");

            _earlyExpires = value;
        }
    }

    /// <summary>
    /// The server address of the callback request
    /// </summary>
    public string CallbackUrl { get; set; }

    /// <summary>
    /// The value of the request body when the callback is initiated
    /// </summary>
    public string CallbackBody { get; set; }

    /// <summary>
    /// Large files enable resume after power failure
    /// default: true
    /// </summary>
    public bool EnableResumableUpload { get; set; }

    /// <summary>
    /// large file length
    /// unit: Byte
    /// default: 5GB
    /// </summary>
    public long BigObjectContentLength { get; set; }

    /// <summary>
    /// Gets or sets the size of the part.
    /// </summary>
    /// <value>The size of the part.</value>
    public long? PartSize { get; set; }

    /// <summary>
    /// true: quiet mode; false: detail mode
    /// default: true
    /// </summary>
    public bool Quiet { get; set; }

    public AliyunStorageOptions()
    {
        _durationSeconds = 3600;
        _temporaryCredentialsCacheKey = Const.TEMPORARY_CREDENTIALS_CACHEKEY;
        Quiet = true;
        CallbackBody = "bucket=${bucket}&object=${object}&etag=${etag}&size=${size}&mimeType=${mimeType}";
        EnableResumableUpload = true;
        BigObjectContentLength = 5 * (long)Math.Pow(1024, 3);
    }

    private AliyunStorageOptions(string accessKeyId, string accessKeySecret) : this()
    {
        AccessKeyId = CheckNullOrEmptyAndReturnValue(accessKeyId, nameof(accessKeyId));
        AccessKeySecret = CheckNullOrEmptyAndReturnValue(accessKeySecret, nameof(accessKeySecret));
    }

    public AliyunStorageOptions(string accessKeyId, string accessKeySecret, string endpoint)
        : this(accessKeyId, accessKeySecret)
    {
        string regionId = GetRegionId(endpoint);
        RegionId = CheckNullOrEmptyAndReturnValue(regionId,
            () => throw new ArgumentException("Unrecognized endpoint, failed to get RegionId"));
        Endpoint = CheckNullOrEmptyAndReturnValue(endpoint, nameof(endpoint));
    }

    public AliyunStorageOptions(string accessKeyId, string accessKeySecret, string regionId, EndpointMode mode)
        : this(accessKeyId, accessKeySecret)
    {
        RegionId = CheckNullOrEmptyAndReturnValue(regionId, nameof(regionId));
        Endpoint = GetEndpoint(regionId, mode);
    }

    public AliyunStorageOptions(string accessKeyId, string accessKeySecret, string endpoint, string roleArn, string roleSessionName)
        : this(accessKeyId, accessKeySecret, GetRegionId(endpoint), endpoint, roleArn, roleSessionName)
    {
    }

    public AliyunStorageOptions(string accessKeyId, string accessKeySecret, string regionId, EndpointMode mode, string roleArn,
        string roleSessionName)
        : this(accessKeyId, accessKeySecret, regionId, GetEndpoint(regionId, mode), roleArn, roleSessionName)
    {
    }

    public AliyunStorageOptions(string accessKeyId, string accessKeySecret, string regionId, string endpoint, string roleArn,
        string roleSessionName)
        : this(accessKeyId, accessKeySecret)
    {
        RegionId = CheckNullOrEmptyAndReturnValue(regionId, nameof(regionId));
        Endpoint = endpoint;
        RoleArn = CheckNullOrEmptyAndReturnValue(roleArn, nameof(roleArn));
        RoleSessionName = CheckNullOrEmptyAndReturnValue(roleSessionName, nameof(roleSessionName));
    }

    public AliyunStorageOptions SetPolicy(string policy)
    {
        Policy = policy;
        return this;
    }

    public AliyunStorageOptions SetTemporaryCredentialsCacheKey(string temporaryCredentialsCacheKey)
    {
        TemporaryCredentialsCacheKey = temporaryCredentialsCacheKey;
        return this;
    }

    public AliyunStorageOptions SetDurationSeconds(int durationSeconds)
    {
        DurationSeconds = durationSeconds;
        return this;
    }

    public AliyunStorageOptions SetEarlyExpires(int earlyExpires)
    {
        EarlyExpires = earlyExpires;
        return this;
    }

    internal string CheckNullOrEmptyAndReturnValue(string? parameter, string parameterName)
    {
        if (string.IsNullOrEmpty(parameter))
            throw new ArgumentException($"{parameterName} cannot be null and empty string");

        return parameter;
    }

    private string CheckNullOrEmptyAndReturnValue(string? parameter, Action error)
    {
        if (string.IsNullOrEmpty(parameter))
            error.Invoke();

        return parameter!;
    }

    private static string GetEndpoint(string regionId, EndpointMode mode)
        => regionId + (mode == EndpointMode.Public ? Const.PUBLIC_ENDPOINT_DOMAIN_SUFFIX : Const.INTERNAL_ENDPOINT_SUFFIX);

    private static string GetRegionId(string endpoint)
    {
        if (endpoint.EndsWith(Const.INTERNAL_ENDPOINT_SUFFIX, StringComparison.OrdinalIgnoreCase))
            return endpoint.Remove(endpoint.Length - Const.INTERNAL_ENDPOINT_SUFFIX.Length);

        return endpoint.Remove(endpoint.Length - Const.PUBLIC_ENDPOINT_DOMAIN_SUFFIX.Length);
    }
}
