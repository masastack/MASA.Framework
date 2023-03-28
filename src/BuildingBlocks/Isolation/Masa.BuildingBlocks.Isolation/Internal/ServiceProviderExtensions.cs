// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.BuildingBlocks.RulesEngine")]
[assembly: InternalsVisibleTo("Masa.BuildingBlocks.Storage.ObjectStorage")]
[assembly: InternalsVisibleTo("Masa.BuildingBlocks.SearchEngine.AutoComplete")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

internal static class ServiceProviderExtensions
{
    public static bool EnableIsolation(this IServiceProvider serviceProvider)
        => serviceProvider.GetService<IOptions<IsolationOptions>>()?.Value.Enable ?? false;
}
