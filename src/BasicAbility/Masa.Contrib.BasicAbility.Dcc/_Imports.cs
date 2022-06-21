// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Configuration;
global using Masa.BuildingBlocks.Configuration.Options;
global using Masa.Contrib.BasicAbility.Dcc.Internal;
global using Masa.Contrib.BasicAbility.Dcc.Internal.Model;
global using Masa.Contrib.BasicAbility.Dcc.Internal.Options;
global using Masa.Contrib.BasicAbility.Dcc.Internal.Parser;
global using Masa.Contrib.BasicAbility.Dcc.Options;
global using Masa.Utils.Caching.Core.DependencyInjection;
global using Masa.Utils.Caching.Core.Models;
global using Masa.Utils.Caching.DistributedMemory.DependencyInjection;
global using Masa.Utils.Caching.DistributedMemory.Interfaces;
global using Masa.Utils.Caching.Redis.Extensions;
global using Masa.Utils.Caching.Redis.Models;
global using Masa.Utils.Caller.Core;
global using Masa.Utils.Caller.HttpClient;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Dynamic;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using static Masa.Contrib.BasicAbility.Dcc.Internal.Constants;
