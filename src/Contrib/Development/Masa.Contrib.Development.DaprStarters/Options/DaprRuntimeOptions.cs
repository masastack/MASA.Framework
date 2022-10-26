// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.Contrib.Development.DaprStarters;

public class DaprRuntimeOptions
{
    [JsonPropertyName("appId")]
    public string AppId { get; set; } = default!;

    [JsonPropertyName("httpPort")]
    public ushort HttpPort { get; set; } = default!;

    [JsonPropertyName("grpcPort")]
    public ushort GrpcPort { get; set; } = default!;

    [JsonPropertyName("appPort")]
    public ushort AppPort { get; set; } = default!;

    [JsonPropertyName("metricsEnabled")]
    public bool MetricsEnabled { get; set; } = default!;

    [JsonPropertyName("command")]
    public string Command { get; set; } = default!;

    [JsonPropertyName("pid")]
    public int PId { get; set; } = default!;
}
