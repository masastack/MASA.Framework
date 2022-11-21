// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;

public class TraceHttpResponseDto
{
    public virtual string Name { get; set; }

    public virtual int Status { get; set; }

    public virtual string Method { get; set; }

    public virtual string Url { get; set; }

    public virtual string Target { get; set; }

    public virtual string Host { get; set; }

    public virtual string Scheme { get; set; }

    public virtual int StatusCode { get; set; }

    public virtual string Flavor { get; set; }

    public virtual string UserAgent { get; set; }

    public virtual int RequestContentLength { get; set; }

    public virtual int RequestContentLengthUncompressed { get; set; }

    public virtual int ResponseContentLength { get; set; }

    public virtual int ResponseContentLengthUncompressed { get; set; }

    public virtual int RetryCount { get; set; }

    public virtual string PeerIp { get; set; }

    public virtual int? PeerPort { get; set; }

    public Dictionary<string, IEnumerable<string>> RequestHeaders { get; set; }

    public Dictionary<string, IEnumerable<string>> ReponseHeaders { get; set; }

    /// <summary>
    /// http client
    /// </summary>   
    public virtual string PeerName { get; set; }

    #region http server
    public virtual string ServerName { get; set; }

    public virtual string Route { get; set; }

    public virtual string ClientIp { get; set; }
    #endregion
}
