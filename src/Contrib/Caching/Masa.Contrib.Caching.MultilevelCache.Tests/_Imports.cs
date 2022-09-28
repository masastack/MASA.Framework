// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Caching;
global using Masa.Contrib.Caching.Distributed.StackExchangeRedis;
global using Masa.Contrib.Caching.MultilevelCache.Tests.Infrastructure;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using NSubstitute;
