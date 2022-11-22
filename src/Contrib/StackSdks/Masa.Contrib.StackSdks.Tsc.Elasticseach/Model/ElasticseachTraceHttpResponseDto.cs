// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticseach.Model;

internal class ElasticseachTraceHttpResponseDto : TraceHttpResponseDto
{
    [JsonPropertyName("http.method")]
    public override string Method { get; set; }

    [JsonPropertyName("http.url")]
    public override string Url { get; set; }

    [JsonPropertyName("http.target")]
    public override string Target { get; set; }

    [JsonPropertyName("http.host")]
    public override string Host { get; set; }

    [JsonPropertyName("http.scheme")]
    public override string Scheme { get; set; }

    [JsonPropertyName("http.status_code")]
    public override int StatusCode { get; set; }

    [JsonPropertyName("http.flavor")]
    public override string Flavor { get; set; }

    [JsonPropertyName("http.user_agent")]
    public override string UserAgent { get; set; }

    [JsonPropertyName("http.request_content_length")]
    public override int RequestContentLength { get; set; }

    [JsonPropertyName("http.request_content_length_uncompressed")]
    public override int RequestContentLengthUncompressed { get; set; }

    [JsonPropertyName("http.response_content_length")]
    public override int ResponseContentLength { get; set; }

    [JsonPropertyName("http.response_content_length_uncompressed")]
    public override int ResponseContentLengthUncompressed { get; set; }

    [JsonPropertyName("http.retry_count")]
    public override int RetryCount { get; set; }

    [JsonPropertyName("net.peer.ip")]
    public override string PeerIp { get; set; }

    [JsonPropertyName("net.peer.port")]
    public override int? PeerPort { get; set; }

    [JsonPropertyName("net.peer.name")]
    public override string PeerName { get; set; }

    #region http server

    [JsonPropertyName("http.server_name")]
    public override string ServerName { get; set; }

    [JsonPropertyName("http.route")]
    public override string Route { get; set; }

    [JsonPropertyName("http.client_ip")]
    public override string ClientIp { get; set; }

    #endregion
}
