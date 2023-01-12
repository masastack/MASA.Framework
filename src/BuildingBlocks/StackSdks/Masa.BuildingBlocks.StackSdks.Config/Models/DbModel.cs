// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Config.Models;

public class DbModel
{

    public string Server { get; set; }

    public int Port { get; set; } = 1433;

    public string Database { get; set; }

    public string UserId { get; set; }

    public string Password { get; set; }

    public string ToString(string datebase)
    {
        return $"Server={Server},{Port};Database={datebase};User Id={UserId};Password={Password};";
    }
}
