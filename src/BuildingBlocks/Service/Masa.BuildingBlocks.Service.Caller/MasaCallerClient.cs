// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public class MasaCallerClient
{
    public IServiceProvider ServiceProvider { get; }

    public Func<IServiceProvider, IRequestMessage>? RequestMessageFactory { get; set; } = null;

    public Func<IServiceProvider, IResponseMessage>? ResponseMessageFactory { get; set; } = null;

    protected MasaCallerClient(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}
