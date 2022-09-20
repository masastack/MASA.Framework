// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Contracts.EFCore.Tests.Models;

public class LogItem
{
    public int Level { get; set; }

    public string Message { get; set; }

    public DateTime CreateTime { get; set; }

    public LogItem()
    {
        CreateTime = DateTime.Now;
    }
}
