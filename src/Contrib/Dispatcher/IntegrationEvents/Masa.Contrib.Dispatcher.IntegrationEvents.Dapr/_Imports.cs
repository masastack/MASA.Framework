// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Dapr.Client;
global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Ddd.Domain.Events;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents.Logs;
global using Masa.Contrib.Dispatcher.IntegrationEvents;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Internal;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Options;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using System.Reflection;
