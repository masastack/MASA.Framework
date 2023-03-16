// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

public class IsolationOptions
{
    public string? SectionName { get; set; }

    public bool Enable => !string.IsNullOrWhiteSpace(SectionName);
}
