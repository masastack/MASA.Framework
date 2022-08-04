// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Contracts.EFCore.Options;

public class FilterOptions
{
    /// <summary>
    /// enable soft delete
    /// default: true
    /// If you are sure that you do not need to use soft delete in the project, you can change to false
    /// IDataFilter does not support ISoftDelete when soft delete is disabled
    /// </summary>
    public bool EnableSoftDelete { get; set; } = true;
}
