// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Model;

public class LableValuesRequest
{
    private readonly string[] _match = new string[1];

    /// <summary>
    /// it for the parameter `match` whitch is IEnumerable<string>
    /// and in this case only support single mode,for example:
    /// Match="up", last `Matches` result is (IEnumerable<string>){"up"}
    /// </summary>
    [JsonIgnore]
    public string Match { get { return _match[0]; } set { _match[0] = value; } }

    /// <summary>
    /// for parameter transform
    /// </summary>
    [JsonPropertyName("match")]
    public IEnumerable<string> Matches { get { return _match; } }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }
}
