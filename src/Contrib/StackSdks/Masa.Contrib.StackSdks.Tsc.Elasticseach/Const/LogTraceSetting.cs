// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticseach.Const;

public class LogTraceSetting
{
    internal string IndexName { get; private set; }

    public string Timestamp { get; private set; }

    internal bool IsIndependent { get; private set; }

    internal ElasticseacherMappingResponseDto[] Mappings { get; private set; }

    internal LogTraceSetting(string indexName, IEnumerable<ElasticseacherMappingResponseDto> mappings, bool isIndependent = false, string timestamp = "@timestamp")
    {
        if (!string.IsNullOrEmpty(IndexName) || string.IsNullOrEmpty(indexName) || mappings == null)
            return;

        IsIndependent = isIndependent;
        IndexName = indexName;
        Mappings = mappings.ToArray();
        Timestamp = timestamp;
    }
}
