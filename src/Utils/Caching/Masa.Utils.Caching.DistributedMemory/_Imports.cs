// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.Utils.Caching.Core.DependencyInjection;
global using Masa.Utils.Caching.Core.Helpers;
global using Masa.Utils.Caching.Core.Interfaces;
global using Masa.Utils.Caching.Core.Models;
global using Masa.Utils.Caching.DistributedMemory.Interfaces;
global using Masa.Utils.Caching.DistributedMemory.Models;
global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Options;
global using System.Collections.Concurrent;
