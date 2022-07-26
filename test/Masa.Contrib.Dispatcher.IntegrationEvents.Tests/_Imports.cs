// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Data.Options;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents.Logs;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Options;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Processor;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Tests.Events;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Tests.Infrastructure;
global using Masa.Utils.Models.Config;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Logging.Abstractions;
global using Microsoft.Extensions.Options;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Moq;
global using System.Data.Common;
global using System.Reflection;
