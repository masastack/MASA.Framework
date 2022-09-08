// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Caching;
global using Masa.BuildingBlocks.Configuration;
global using Masa.BuildingBlocks.Configuration.Options;
global using Masa.BuildingBlocks.Service.Caller;
global using Masa.BuildingBlocks.Service.Caller.Options;
global using Masa.Contrib.Caching.Distributed.StackExchangeRedis;
global using Masa.Contrib.Caching.MultilevelCache;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal.Model;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal.Parser;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;
global using Masa.Contrib.Service.Caller;
global using Masa.Contrib.Service.Caller.HttpClient;
global using Masa.Utils.Security.Cryptography;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using System.Collections.Concurrent;
global using System.Data;
global using System.Diagnostics;
global using System.Dynamic;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using YamlDotNet.Serialization;
global using static Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal.Constants;
