// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore.Tests.Scenes.Isolation")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore;

internal interface IConnectionStringProviderWrapper : IConnectionStringProvider
{

}
