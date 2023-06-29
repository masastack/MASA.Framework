// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticsearch.Constants;

public static class ElasticConstant
{
    internal const string LOG_CALLER_CLIENT_NAME = "masa.contrib.stacksdks.tsc.log.elasticseach.log";
    internal const string TRACE_CALLER_CLIENT_NAME = "masa.contrib.stacksdks.tsc.log.elasticseach.trace";
    internal const string DEFAULT_CALLER_CLIENT_NAME = "masa.contrib.stacksdks.tsc.log.elasticseach.all";

    private const string TIMESTAMP = "@timestamp";
    public static string TraceId => "TraceId";
    public static string ParentId => "ParentSpanId";
    public static string SpanId => "SpanId";
    public static string ServiceName => "Resource.service.name";
    public static string ServiceInstance => "Resource.service.instance.id";
    public static string NameSpace => "Resource.service.namespace";
    public static string Endpoint => "Attributes.http.url";

    internal static int MaxRecordCount { get; private set; } = 10000;

    public static LogTraceSetting Log { get; private set; }

    public static LogTraceSetting Trace { get; private set; }

    internal static void InitLog(string indexName, bool isIndependent = false)
    {
        if (string.IsNullOrEmpty(indexName))
            return;
        Log = new LogTraceSetting(indexName, isIndependent, TIMESTAMP);
    }

    internal static void InitTrace(string indexName, bool isIndependent = false)
    {
        if (string.IsNullOrEmpty(indexName))
            return;
        Trace = new LogTraceSetting(indexName, isIndependent, TIMESTAMP);
    }
}
