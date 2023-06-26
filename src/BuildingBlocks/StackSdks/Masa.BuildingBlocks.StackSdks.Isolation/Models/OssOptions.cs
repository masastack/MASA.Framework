// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Isolation.Models;

public class OssOptions
{
    public string AccessId { get; set; } = "";

    public string AccessSecret { get; set; } = "";

    public string Bucket { get; set; } = "";

    public string Endpoint { get; set; } = "";

    public string RoleArn { get; set; } = "";

    public string RoleSessionName { get; set; } = "";

    public string RegionId { get; set; } = "";
}
