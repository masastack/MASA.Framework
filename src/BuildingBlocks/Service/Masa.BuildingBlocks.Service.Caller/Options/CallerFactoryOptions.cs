// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public class CallerFactoryOptions : MasaFactoryOptions<CallerRelationOptions>
{
    public JsonSerializerOptions? JsonSerializerOptions { get; set; }

    public string? RequestIdKey { get; set; }
}
