// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Config;

public interface IMasaStackConfig
{
    public RedisModel RedisModel { get; }

    public ElasticModel ElasticModel { get; }

    public bool IsDemo { get; }

    public string TlsName { get; }

    public string Version { get; }

    public string Environment { get; }

    public string Namespace { get; }

    public string Cluster { get; }

    public string OtlpUrl { get; }

    public string DomainName { get; }

    string GetValue(string key);

    void SetValue(string key, string value);

    public List<string> ProjectList();
}
