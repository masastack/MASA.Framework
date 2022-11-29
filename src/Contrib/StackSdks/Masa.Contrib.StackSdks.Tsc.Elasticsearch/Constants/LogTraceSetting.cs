// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticsearch.Constants;

public class LogTraceSetting
{
    internal string IndexName { get; private set; }

    public string Timestamp { get; private set; }

    internal bool IsIndependent { get; private set; }

    internal Lazy<ElasticseacherMappingResponseDto[]> Mappings { get; set; }

    internal LogTraceSetting(string indexName, bool isIndependent = false, string timestamp = "@timestamp")
    {
        if (!string.IsNullOrEmpty(IndexName) || string.IsNullOrEmpty(indexName))
            return;

        IndexName = indexName;
        Timestamp = timestamp;
        IsIndependent = isIndependent;
    }
}
