// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Prometheus.Model;

public class MetricMetaQueryRequest
{
    /// <summary>
    /// search metric name ,use full match
    /// </summary>
    public string Metric { get; set; }

    /// <summary>
    /// default all ,if set 0, then can't return any data
    /// </summary>
    public int? Limit { get; set; }
}
