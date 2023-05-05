// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Caching;
global using Masa.BuildingBlocks.Isolation;
global using Masa.Contrib.Caching.Distributed.StackExchangeRedis;
global using Masa.Utils.Caching.Memory;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Options;
global using StackExchange.Redis;
global using System.IO.Compression;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Text.Json;
