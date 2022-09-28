// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Cache.Caches;
global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;
global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Enums;
global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Models;
global using Masa.Contrib.Authentication.OpenIdConnect.Cache;
global using Masa.Contrib.Authentication.OpenIdConnect.Cache.Caches;
global using Masa.Contrib.Authentication.OpenIdConnect.Cache.Models;
global using Masa.Contrib.Authentication.OpenIdConnect.Cache.Utils;
global using Masa.Utils.Caching.Core.Models;
global using Masa.Utils.Caching.DistributedMemory.DependencyInjection;
global using Masa.Utils.Caching.DistributedMemory.Interfaces;
global using Masa.Utils.Caching.Redis.Models;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using System.Diagnostics.CodeAnalysis;
