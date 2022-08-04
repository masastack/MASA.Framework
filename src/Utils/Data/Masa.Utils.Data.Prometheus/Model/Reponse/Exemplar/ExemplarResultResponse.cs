// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Prometheus.Model;

public class ExemplarResultResponse : ResultBaseResponse
{
    public IEnumerable<ExemplarDataModel>? Data { get; set; }
}
