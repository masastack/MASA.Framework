// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Model;

public class ValuesRequest
{
    public string Match { get; set; }

    /// <summary>
    /// for parameter `match` child,  for example
    /// Match = "up", Lables=new string[]{ "instance=\"k8s-hz-001\"" }, last result is 'up{instance="k8s-hz-001"} for `Match`'
    /// </summary>
    [JsonIgnore]
    public IEnumerable<string>? Lables { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }
}
