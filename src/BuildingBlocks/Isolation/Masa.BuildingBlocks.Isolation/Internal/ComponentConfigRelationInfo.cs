// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

internal class ComponentConfigRelationInfo
{
    public Type ComponentConfigType { get; set; }

    public string SectionName { get; set; }

    public object? Data { get; set; }
}
