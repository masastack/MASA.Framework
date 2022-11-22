// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;

public class TraceNetResponseDto
{
    public virtual string Transport { get; set; }

    public virtual string PeerIp { get; set; }

    public virtual int PeerPort { get; set; }

    public virtual string PeerName { get; set; }

    public virtual string HostIp { get; set; }

    public virtual int HostPort { get; set; }

    public virtual string HostName { get; set; }

    public virtual string HostConnectType { get; set; }

    public virtual string HostConnectSubtype { get; set; }

    public virtual string CarrierName { get; set; }

    public virtual string CarrierMCC { get; set; }

    public virtual string CarrierMNC { get; set; }

    public virtual string CarrierICC { get; set; }

    public virtual string PeerService { get; set; }
}
