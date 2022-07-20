// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.BasicAbility.Tsc;
global using Masa.BuildingBlocks.BasicAbility.Tsc.Model;
global using Masa.BuildingBlocks.BasicAbility.Tsc.Service;
global using Masa.Contrib.BasicAbility.Tsc;
global using Masa.Contrib.BasicAbility.Tsc.Service;
global using Masa.Utils.Caller.Core;
global using Masa.Utils.Caller.HttpClient;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.DependencyInjection;
global using OpenTelemetry.Contrib.Instrumentation.ElasticsearchClient;
global using OpenTelemetry.Contrib.Instrumentation.EntityFrameworkCore;
global using OpenTelemetry.Instrumentation.AspNetCore;
global using OpenTelemetry.Instrumentation.Http;
global using OpenTelemetry.Instrumentation.StackExchangeRedis;
global using OpenTelemetry.Logs;
global using OpenTelemetry.Metrics;
global using OpenTelemetry.Trace;
global using StackExchange.Redis;
global using System.Diagnostics;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Runtime.CompilerServices;
global using System.Text;
