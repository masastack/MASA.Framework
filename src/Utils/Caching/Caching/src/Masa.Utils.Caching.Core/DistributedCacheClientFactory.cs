// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Core;

public abstract class DistributedCacheClientFactory<TOptions> : IDistributedCacheClientFactory
{
    private readonly IOptionsMonitor<TOptions> _optionsMonitor;

    private readonly ConcurrentDictionary<string, Lazy<IDistributedCacheClient>> _clients;
    private readonly Func<string, Lazy<IDistributedCacheClient>> _clientFactory;

    public DistributedCacheClientFactory(IOptionsMonitor<TOptions> optionsMonitor)
    {
        if (optionsMonitor == null)
        {
            throw new ArgumentNullException(nameof(optionsMonitor));
        }

        _optionsMonitor = optionsMonitor;

        _clients = new ConcurrentDictionary<string, Lazy<IDistributedCacheClient>>();

        _clientFactory = (name) =>
        {
            return new Lazy<IDistributedCacheClient>(() =>
            {
                return CreateClientHandler(name);
            });
        };
    }

    // <inherit />
    public IDistributedCacheClient CreateClient(string name)
    {
        name ??= string.Empty;

        var client = _clients.GetOrAdd(name, _clientFactory);

        return client.Value;
    }

    internal protected abstract IDistributedCacheClient CreateClientHandler(string name);

    protected TOptions GetOptions(string name)
    {
        return _optionsMonitor.Get(name);
    }
}
