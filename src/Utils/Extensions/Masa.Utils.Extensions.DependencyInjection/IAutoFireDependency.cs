// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Automatic trigger injection, After the service is added, it will be obtained once
/// Need to be used with ISingletonDependency, IScopedDependency, ISingletonDependency
/// </summary>
public interface IAutoFireDependency
{
}
