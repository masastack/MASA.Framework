// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Log.Elasticseach.Const;

public static class ElasticConst
{
    internal const string LOG_CALLER_CLIENT_NAME = "masa.contrib.stacksdks.tsc.log.elasticseach.log";
    internal const string TRACE_CALLER_CLIENT_NAME = "masa.contrib.stacksdks.tsc.log.elasticseach.trace";
    internal const string DEFAULT_CALLER_CLIENT_NAME = "masa.contrib.stacksdks.tsc.log.elasticseach.all";

    public static string TraceId => "TraceId";
    public static string ParentId => "ParentSpanId";
    public static string SpanId => "SpanId";
    public static string ServiceName => "Resource.service.name";
    public static string ServiceInstance => "Resource.service.instance.id";
    public static string NameSpace => "Resource.service.namespace";
    public static string Endpoint => "Attributes.http.target";

    internal static int MaxRecordCount { get; private set; } = 10000;

    public static class Log
    {
        internal static string IndexName { get; private set; }

        public static string Timestamp => "@timestamp";

        internal static bool IsIndependent { get; private set; }

        internal static ElasticseacherMappingResponseDto[] Mappings { get; private set; }

        internal static void Init(string indexName, IEnumerable<ElasticseacherMappingResponseDto> mappings, bool isIndependent = false)
        {
            if (!string.IsNullOrEmpty(IndexName) || string.IsNullOrEmpty(indexName))
                return;

            IsIndependent = isIndependent;
            IndexName = indexName;
            if (mappings == null || !mappings.Any())
                Mappings = new ElasticseacherMappingResponseDto[] {
                    new ElasticseacherMappingResponseDto{
                        Name = Timestamp,
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
            else
                Mappings = mappings.ToArray();
        }
    }

    public static class Trace
    {
        internal static string IndexName { get; private set; }

        public static string Timestamp => "@timestamp";

        internal static bool IsIndependent { get; private set; }

        internal static ElasticseacherMappingResponseDto[] Mappings { get; private set; }

        internal static void Init(string indexName, IEnumerable<ElasticseacherMappingResponseDto> mappings, bool isIndependent = false)
        {
            if (!string.IsNullOrEmpty(IndexName) || string.IsNullOrEmpty(indexName))
                return;

            IsIndependent = isIndependent;
            IndexName = indexName;
            if (mappings == null || !mappings.Any())
                Mappings = new ElasticseacherMappingResponseDto[] {
                    new ElasticseacherMappingResponseDto{
                        Name=Timestamp,
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
            else
                Mappings = mappings.ToArray();
        }
    }
}
