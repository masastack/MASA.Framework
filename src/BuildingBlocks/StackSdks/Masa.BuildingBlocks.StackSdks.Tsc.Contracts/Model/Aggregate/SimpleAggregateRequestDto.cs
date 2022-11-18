// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model.Aggregate;

public class SimpleAggregateRequestDto : BaseRequestDto
{
    public string Name { get; set; }

    public string Alias { get; set; }   
    
    public AggregateTypes Type { get; set; }

    public int MaxCount { get; set; }

    /// <summary>
    /// https://www.elastic.co/guide/en/elasticsearch/reference/7.17/search-aggregations-bucket-datehistogram-aggregation.html
    /// </summary>
    public string Interval { get; set; }
}
