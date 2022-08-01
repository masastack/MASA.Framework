// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Scheduler.Model;

public class SchedulerJobHttpConfig
{
    public HttpMethods HttpMethod { get; set; }

    public string RequestUrl { get; set; } = string.Empty;

    public List<KeyValuePair<string, string>> HttpParameters { get; set; } = new();

    public List<KeyValuePair<string, string>> HttpHeaders { get; set; } = new();

    public string HttpBody { get; set; } = string.Empty;

    public HttpVerifyTypes HttpVerifyType { get; set; }

    public string VerifyContent { get; set; } = string.Empty;
}
