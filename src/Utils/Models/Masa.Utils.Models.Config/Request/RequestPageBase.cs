// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Utils.Models;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class RequestPageBase
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
