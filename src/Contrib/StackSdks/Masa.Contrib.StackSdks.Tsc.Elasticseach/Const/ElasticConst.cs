// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticseach.Const;

public static class ElasticConst
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
    public static string Endpoint => "Attributes.http.target";

    internal static int MaxRecordCount { get; private set; } = 10000;

    public static LogTraceSetting Log { get; private set; }

    public static LogTraceSetting Trace { get; private set; }

    internal static void InitLog(string indexName, IEnumerable<ElasticseacherMappingResponseDto> mappings, bool isIndependent = false)
    {
        if (Log != null || string.IsNullOrEmpty(indexName))
            return;
        if (mappings == null || !mappings.Any())
            mappings = new ElasticseacherMappingResponseDto[] {
                    new ElasticseacherMappingResponseDto{
                        Name = TIMESTAMP,
                        Type ="date"
                    },
                    new ElasticseacherMappingResponseDto{
                        Name=ServiceInstance,
                        Type ="text",
                         IsKeyword=true,
                    },
                    new ElasticseacherMappingResponseDto{
                        Name=ServiceName,
                        Type ="text",
                         IsKeyword=true,
                    },
                    new ElasticseacherMappingResponseDto{
                        Name=NameSpace,
                        Type ="text",
                        IsKeyword=true,
                    },
                    new ElasticseacherMappingResponseDto{
                        Name="Resource.service.version",
                        Type ="text",
                        IsKeyword=true,
                    },
                    new ElasticseacherMappingResponseDto{
                        Name=SpanId,
                        Type ="text",
                        IsKeyword=true,
                    },
                    new ElasticseacherMappingResponseDto{
                        Name=TraceId,
                        Type ="text",
                        IsKeyword=true,
                    },
                    new ElasticseacherMappingResponseDto{
                        Name=ParentId,
                        Type ="text",
                        IsKeyword=true,
                    },
                    new ElasticseacherMappingResponseDto{
                        Name="SeverityNumber",
                        Type ="number"
                    },
                    new ElasticseacherMappingResponseDto{
                        Name="TraceFlags",
                        Type ="number"
                    },
                    new ElasticseacherMappingResponseDto{
                        Name="SeverityText",
                        Type ="text",
                        IsKeyword=true,
                    }
                };
        Log = new LogTraceSetting(indexName, mappings, isIndependent, TIMESTAMP);
    }

    internal static void InitTrace(string indexName, IEnumerable<ElasticseacherMappingResponseDto> mappings, bool isIndependent = false)
    {
        if (Trace != null || string.IsNullOrEmpty(indexName))
            return;

        if (mappings == null || !mappings.Any())
            mappings = new ElasticseacherMappingResponseDto[] {
                    new ElasticseacherMappingResponseDto{
                        Name=TIMESTAMP,
                        Type ="date"
                    },
                   new ElasticseacherMappingResponseDto{
                        Name="EndTimestamp",
                        Type ="date"
                    },
                    new ElasticseacherMappingResponseDto{
                        Name="Name",
                        Type ="text",
                        IsKeyword=true
                    },
                    new ElasticseacherMappingResponseDto{
                        Name=ServiceInstance,
                        Type ="text",
                        IsKeyword=true,
                    },
                    new ElasticseacherMappingResponseDto{
                        Name=ServiceName,
                        Type ="text",
                        IsKeyword=true,
                    },
                    new ElasticseacherMappingResponseDto{
                        Name=NameSpace,
                        Type ="text",
                        IsKeyword=true,
                    },
                    new ElasticseacherMappingResponseDto{
                        Name="Resource.service.version",
                        Type ="text",
                        IsKeyword=true,
                    },
                    new ElasticseacherMappingResponseDto{
                        Name=SpanId,
                        Type ="text",
                        IsKeyword=true,
                    },
                    new ElasticseacherMappingResponseDto{
                        Name=TraceId,
                        Type ="text",
                        IsKeyword=true,
                    },
                    new ElasticseacherMappingResponseDto{
                        Name=ParentId,
                        Type ="text",
                        IsKeyword=true,
                    },
                    new ElasticseacherMappingResponseDto{
                        Name="TraceFlags",
                        Type ="number"
                    },
                    new ElasticseacherMappingResponseDto{
                        Name=Endpoint,
                        Type ="text",
                        IsKeyword=true
                    }
                };
        Trace = new LogTraceSetting(indexName, mappings, isIndependent, TIMESTAMP);
    }
}
