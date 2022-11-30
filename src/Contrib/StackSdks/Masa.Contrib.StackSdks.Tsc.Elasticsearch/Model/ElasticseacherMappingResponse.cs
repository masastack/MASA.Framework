// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticsearch.Model;

public class ElasticseacherMappingResponseDto : MappingResponseDto
{
    /// <summary>
    /// if has keyword is true ,else false
    /// </summary>
    public bool? IsKeyword { get; set; }

    /// <summary>
    /// keyword query max length
    /// </summary>
    public int? MaxLenth { get; set; }
}
