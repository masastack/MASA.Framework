// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticsearch.Tests;

public static class StaticConfig
{
    internal const string LOG_INDEX_NAME = "test_logs";
    internal const string HOST = "http://localhost:9200";
    internal const string TRACE_INDEX_NAME = "test_traces";
    private static IConfiguration Configuration;

    public static IConfiguration InitConfiguration()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("testData.json")
            .Build();
        return config;
    }

    public static string GetJson(string name)
    {
        if (Configuration == null)
            Configuration = InitConfiguration();
        return Configuration.GetSection(name).Get<string>();
    }

    public static HttpRequestMessage CreateMessage(string indexName, HttpMethod method, string? message=null)
    {
        var result = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new Uri($"/{indexName}", UriKind.Relative)
        };
        if (!string.IsNullOrEmpty(message))
            result.Content = new StringContent(message, Encoding.UTF8, "application/json");
        return result;
    }
}
