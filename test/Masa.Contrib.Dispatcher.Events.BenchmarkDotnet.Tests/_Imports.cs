// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using BenchmarkDotNet.Attributes;
global using BenchmarkDotNet.Configs;
global using BenchmarkDotNet.Engines;
global using BenchmarkDotNet.Jobs;
global using BenchmarkDotNet.Running;
global using BenchmarkDotNet.Validators;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.Contrib.Dispatcher.Events.BenchmarkDotnet.Tests.Extensions.EventHandlers;
global using Masa.Contrib.Dispatcher.Events.BenchmarkDotnet.Tests.Extensions.Events;
global using Masa.Contrib.Dispatcher.Events.Enums;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using System;
global using System.Threading.Tasks;

