// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Caching;
global using Masa.BuildingBlocks.Data;
global using Masa.Contrib.Caching.Distributed.StackExchangeRedis;
global using Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;
global using Masa.Contrib.Data.IdGenerator.Snowflake.Internal;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using StackExchange.Redis;
global using System;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
