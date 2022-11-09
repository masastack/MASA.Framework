// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
global using Masa.Contrib.Dispatcher.Events.Enums;
global using Masa.Contrib.Dispatcher.Events.Tests.Events;
global using Masa.Contrib.Dispatcher.Events.Tests.Middleware;
global using Masa.Contrib.Dispatcher.Events.Tests.Scenes.CheckMethodsParameterNotNull.Events;
global using Masa.Contrib.Dispatcher.Events.Tests.Scenes.CheckMethodsParameterType.Events;
global using Masa.Contrib.Dispatcher.Events.Tests.Scenes.CheckMethodsType.Events;
global using Masa.Contrib.Dispatcher.Events.Tests.Scenes.HandlerOrder.Events;
global using Masa.Contrib.Dispatcher.Events.Tests.Scenes.OnlyCancelHandler.Events;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Moq;
global using System.Reflection;
