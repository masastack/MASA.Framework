// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.Contrib.StackSdks.Tsc;
global using Masa.Contrib.StackSdks.Tsc.OpenTelemetry;
global using Masa.Contrib.StackSdks.Tsc.OpenTelemetry.Metric.Instrumentation.Http;
global using Masa.Contrib.StackSdks.Tsc.OpenTelemetry.Tracing.Handler;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using OpenTelemetry;
global using OpenTelemetry.Instrumentation.AspNetCore;
global using OpenTelemetry.Instrumentation.ElasticsearchClient;
global using OpenTelemetry.Instrumentation.EntityFrameworkCore;
global using OpenTelemetry.Instrumentation.Http;
global using OpenTelemetry.Instrumentation.StackExchangeRedis;
global using OpenTelemetry.Logs;
global using OpenTelemetry.Metrics;
global using OpenTelemetry.Resources;
global using OpenTelemetry.Trace;
global using StackExchange.Redis;
global using System;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.Diagnostics.Metrics;
global using System.IO;
global using System.Linq;
global using System.Net;
global using System.Net.Http;
global using System.Net.Http.Headers;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Threading.Tasks;
