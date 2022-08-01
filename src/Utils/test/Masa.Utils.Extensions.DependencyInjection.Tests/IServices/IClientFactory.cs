﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DependencyInjection.Tests.IServices;

public interface IClientFactory : ISingletonDependency
{
    string GetClientName();
}
