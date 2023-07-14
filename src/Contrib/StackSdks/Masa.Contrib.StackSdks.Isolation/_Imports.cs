// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Authentication.Identity;
global using Masa.BuildingBlocks.Caching;
global using Masa.BuildingBlocks.Configuration;
global using Masa.BuildingBlocks.Configuration.Options;
global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Isolation;
global using Masa.BuildingBlocks.Service.Caller;
global using Masa.BuildingBlocks.StackSdks.Config;
global using Masa.BuildingBlocks.StackSdks.Config.Models;
global using Masa.BuildingBlocks.StackSdks.Isolation;
global using Masa.BuildingBlocks.StackSdks.Isolation.Models;
global using Masa.BuildingBlocks.StackSdks.Pm;
global using Masa.BuildingBlocks.Storage.ObjectStorage;
global using Masa.Contrib.Caching.Distributed.StackExchangeRedis;
global using Masa.Contrib.Caching.MultilevelCache;
global using Masa.Contrib.Configuration;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal.Parser;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;
global using Masa.Contrib.Isolation.MultiEnvironment;
global using Masa.Contrib.Isolation.Parser;
global using Masa.Contrib.StackSdks.Config;
global using Masa.Contrib.Storage.ObjectStorage.Aliyun;
global using Masa.Contrib.Storage.ObjectStorage.Aliyun.Options;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using System.Collections.Concurrent;
global using System.Reflection;
global using System.Text.Json;
