// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DependencyInjection.Tests.Services;

[Dependency(ReplaceServices = true)]
public class CustomizeClientFactory : IClientFactory
{
    public string GetClientName() => nameof(CustomizeClientFactory);
}
