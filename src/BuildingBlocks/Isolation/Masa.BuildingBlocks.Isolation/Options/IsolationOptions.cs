// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation.Options;

public class IsolationOptions
{
    public string TenantId { get; set; }

    public string Environment { get; set; }

    // #region 数据库链接字符串相关信息
    //
    // public string Name { get; set; } = string.Empty;
    //
    // public string ConnectionString { get; set; }
    //
    // #endregion

    public object Module { get; set; }

    /// <summary>
    /// Used to control the configuration with the highest score when multiple configurations are satisfied. The default score is 100
    /// </summary>
    public int Score { get; set; } = 100;
}
