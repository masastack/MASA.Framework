// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Utils.Extensions.DependencyInjection.Tests.Domain.Services;

namespace Masa.Utils.Extensions.DependencyInjection.Tests.Infrastructure.Services;

public class NullCalculateProviderService : ICalculateProviderService
{
    public string Name => string.Empty;
}
