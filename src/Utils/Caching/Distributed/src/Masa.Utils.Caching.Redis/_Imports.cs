// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.Utils.Caching.Core;
global using Masa.Utils.Caching.Core.DependencyInjection;
global using Masa.Utils.Caching.Core.Interfaces;
global using Masa.Utils.Caching.Core.Models;
global using Masa.Utils.Caching.Redis;
global using Masa.Utils.Caching.Redis.Extensions;
global using Masa.Utils.Caching.Redis.Models;
global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Options;
global using StackExchange.Redis;
global using System.Collections;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Dynamic;
global using System.IO.Compression;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using static Masa.Utils.Caching.Redis.Helpers.RedisHelper;
