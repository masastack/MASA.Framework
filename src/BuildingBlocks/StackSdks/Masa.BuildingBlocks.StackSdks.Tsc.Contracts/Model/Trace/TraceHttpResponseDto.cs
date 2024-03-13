// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;

public class TraceHttpResponseDto
{
    public virtual string Name { get; set; }

    public virtual int Status { get; set; }

    [JsonPropertyName("http.method")]
    public virtual string Method { get; set; }

    [JsonPropertyName("http.url")]
    public virtual string Url { get; set; }

    [JsonPropertyName("http.target")]
    public virtual string Target { get; set; }

    [JsonPropertyName("http.host")]
    public virtual string Host { get; set; }

    [JsonPropertyName("http.scheme")]
    public virtual string Scheme { get; set; }

    [JsonPropertyName("http.status_code")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public virtual int StatusCode { get; set; }

    [JsonPropertyName("http.flavor")]
    public virtual string Flavor { get; set; }

    [JsonPropertyName("http.user_agent")]
    public virtual string UserAgent { get; set; }

    [JsonPropertyName("http.request_content_length")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public virtual int RequestContentLength { get; set; }

    [JsonPropertyName("http.request_content_length_uncompressed")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public virtual int RequestContentLengthUncompressed { get; set; }

    public Dictionary<string, IEnumerable<string>> RequestHeaders { get; set; }

    [JsonPropertyName("http.response_content_length")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public virtual int ResponseContentLength { get; set; }

    [JsonPropertyName("http.response_content_length_uncompressed")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public virtual int ResponseContentLengthUncompressed { get; set; }    

    public Dictionary<string, IEnumerable<string>> ReponseHeaders { get; set; }

    [JsonPropertyName("http.retry_count")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public virtual int RetryCount { get; set; }

    [JsonPropertyName("net.peer.ip")]
    public virtual string PeerIp { get; set; }

    [JsonPropertyName("net.peer.port")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public virtual int? PeerPort { get; set; }

    [JsonPropertyName("net.peer.name")]
    public virtual string PeerName { get; set; }

    #region http server

    [JsonPropertyName("http.server_name")]
    public virtual string ServerName { get; set; }

    [JsonPropertyName("http.route")]
    public virtual string Route { get; set; }

    [JsonPropertyName("http.client_ip")]
    public virtual string ClientIp { get; set; }

    #endregion
}
