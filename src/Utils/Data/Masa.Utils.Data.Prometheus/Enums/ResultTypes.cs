// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Prometheus.Enums;

/// <summary>
/// reference https://prometheus.io/docs/prometheus/latest/querying/api/#expression-query-result-formats
/// </summary>
public enum ResultTypes
{
    Matrix = 1,
    Vector,
    Scalar,
    String
}
