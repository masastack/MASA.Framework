// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Service.Caller;
global using Masa.BuildingBlocks.StackSdks.Tsc;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Log;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model.Aggregate;
global using Masa.BuildingBlocks.StackSdks.Tsc.Model;
global using Masa.BuildingBlocks.StackSdks.Tsc.Service;
global using Masa.Contrib.StackSdks.Tsc;
global using Masa.Contrib.StackSdks.Tsc.Metrics.Instrumentation.Http;
global using Masa.Contrib.StackSdks.Tsc.Service;
global using Masa.Contrib.StackSdks.Tsc.Tracing.Handler;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using OpenTelemetry;
global using OpenTelemetry.Contrib.Instrumentation.ElasticsearchClient;
global using OpenTelemetry.Contrib.Instrumentation.EntityFrameworkCore;
global using OpenTelemetry.Instrumentation.AspNetCore;
global using OpenTelemetry.Instrumentation.Http;
global using OpenTelemetry.Instrumentation.StackExchangeRedis;
global using OpenTelemetry.Logs;
global using OpenTelemetry.Metrics;
global using OpenTelemetry.Resources;
global using OpenTelemetry.Trace;
global using StackExchange.Redis;
global using System.Diagnostics;
global using System.Diagnostics.Metrics;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Text.Json;
global using System.Text.RegularExpressions;
