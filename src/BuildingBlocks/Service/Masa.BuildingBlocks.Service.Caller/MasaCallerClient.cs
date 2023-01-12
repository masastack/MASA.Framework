// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public class MasaCallerClient
{
    public Func<IServiceProvider, IRequestMessage>? RequestMessageFactory { get; set; }

    public Func<IServiceProvider, IResponseMessage>? ResponseMessageFactory { get; set; }

    public JsonSerializerOptions? JsonSerializerOptions { get; set; }
}
