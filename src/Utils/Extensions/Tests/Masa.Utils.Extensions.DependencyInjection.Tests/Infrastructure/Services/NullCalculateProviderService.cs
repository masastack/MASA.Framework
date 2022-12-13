// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DependencyInjection.Tests.Infrastructure.Services;

public class NullCalculateProviderService : ICalculateProviderService
{
    public string Name => string.Empty;
}
