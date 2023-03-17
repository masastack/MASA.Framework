// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

internal abstract class LocalMessageDbConnectionStringProviderBase : ILocalMessageDbConnectionStringProvider
{
    private List<string>? _connectionStrings = null;

    public virtual List<string> ConnectionStrings => _connectionStrings ??= GetConnectionStrings();

    protected abstract List<string> GetConnectionStrings();
}
