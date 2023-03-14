// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation;

/// <summary>
/// 需要调整，暂时用来存储隔离性使用状态
/// </summary>
public static class IsolationConfiguration
{
    internal static readonly JsonSerializerOptions DynamicJsonSerializerOptions;

    static IsolationConfiguration()
    {
        DynamicJsonSerializerOptions = new JsonSerializerOptions();
        DynamicJsonSerializerOptions.EnableDynamicTypes();
    }
}
