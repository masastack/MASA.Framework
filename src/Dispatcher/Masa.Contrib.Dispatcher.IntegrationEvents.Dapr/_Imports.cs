// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Dapr.Client;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents.Logs;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Internal;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Options;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Servers;
global using Masa.Utils.Models.Config;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using System.Collections.Concurrent;
global using System.Reflection;
global using System.Text.Json.Serialization;
