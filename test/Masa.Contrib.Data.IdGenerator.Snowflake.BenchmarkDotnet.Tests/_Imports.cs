// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using BenchmarkDotNet.Attributes;
global using BenchmarkDotNet.Engines;
global using BenchmarkDotNet.Jobs;
global using Microsoft.Extensions.DependencyInjection;
global using Masa.BuildingBlocks.Data;
global using Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;
global using Masa.Utils.Caching.Redis.DependencyInjection;
global using Masa.Utils.Caching.Redis.Models;
global using BenchmarkDotNet.Configs;
global using BenchmarkDotNet.Running;
global using BenchmarkDotNet.Validators;
