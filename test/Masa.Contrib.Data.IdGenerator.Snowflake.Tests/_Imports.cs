// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data;
global using Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;
global using Masa.Utils.Caching.Core.Interfaces;
global using Masa.Utils.Caching.Redis;
global using Masa.Utils.Caching.Redis.DependencyInjection;
global using Masa.Utils.Caching.Redis.Models;
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
