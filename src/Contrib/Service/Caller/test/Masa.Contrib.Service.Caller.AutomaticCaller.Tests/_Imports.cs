// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Service.Caller;
global using Masa.BuildingBlocks.Service.Caller.Options;
global using Masa.Contrib.Service.Caller.AutomaticCaller.Tests.Callers;
global using Masa.Contrib.Service.Caller.DaprClient;
global using Masa.Contrib.Service.Caller.HttpClient;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using System.Net;
global using DaprCaller = Masa.Contrib.Service.Caller.AutomaticCaller.Tests.Callers.DaprCaller;
