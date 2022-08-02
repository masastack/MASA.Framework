// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Prometheus.Model;

public class QueryResultInstantVectorResponse
{
    public IDictionary<string, object>? Metric { get; set; }

    public object[]? Value { get; set; }
}
