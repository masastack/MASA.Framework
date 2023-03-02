// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class BackgroundJobInfo
{
    /// <summary>
    /// Job Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Job Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Job Args
    /// </summary>
    public string Args { get; set; }

    public int Times { get; set; } = 0;

    public DateTime CreationTime { get; set; }

    public DateTime NextTryTime { get; set; }

    public DateTime LastTryTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// Is the task invalid?
    /// Invalid after max retries exceeded
    /// </summary>
    public bool IsInvalid { get; set; } = false;
}
