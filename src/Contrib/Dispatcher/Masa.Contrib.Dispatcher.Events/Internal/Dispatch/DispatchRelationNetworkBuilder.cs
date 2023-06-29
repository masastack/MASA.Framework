// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal.Dispatch;

internal class DispatchRelationNetworkBuilder : IDispatchNetworkBuilder
{
    private readonly List<IDispatchNetworkProvider> _providers;

    public DispatchRelationNetworkBuilder() => _providers = new();

    public IDispatchNetworkBuilder Add(IDispatchNetworkProvider provider)
    {
        _providers.Add(provider);
        return this;
    }

    public IDispatchNetworkRoot Build() => new DispatchNetworkRoot(_providers);
}
