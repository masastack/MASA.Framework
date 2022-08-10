// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller.Options;

public class CallerFactoryOptions : MasaFactoryOptions<CallerRelationOptions>
{
    public JsonSerializerOptions? JsonSerializerOptions { get; set; }

    public string? RequestIdKey { get; set; }
}
