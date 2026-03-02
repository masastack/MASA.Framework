// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Dapr.Client;
global using Masa.BuildingBlocks.Configuration;
global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Service.Caller;
global using Masa.BuildingBlocks.StackSdks.Dcc.Contracts.Enum;
global using Masa.BuildingBlocks.StackSdks.Dcc.Contracts.Model;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Contracts;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Contracts.Internal;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Contracts.Internal.Parser;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Dapr.Internal;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Dapr.Options;
global using Masa.Utils.Security.Cryptography;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using StackExchange.Redis;
global using System.Collections.Concurrent;
global using System.Dynamic;
global using System.Text.Json;
global using YamlDotNet.Serialization;
