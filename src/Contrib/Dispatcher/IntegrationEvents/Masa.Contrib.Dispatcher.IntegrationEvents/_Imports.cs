// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Configuration.Options;
global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Ddd.Domain.Events;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents.Logs;
global using Masa.BuildingBlocks.Isolation;
global using Masa.Contrib.Dispatcher.IntegrationEvents;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Options;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Processor;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Servers;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using System.Collections.Concurrent;
global using System.Diagnostics.CodeAnalysis;
global using System.Reflection;
global using System.Runtime.CompilerServices;
