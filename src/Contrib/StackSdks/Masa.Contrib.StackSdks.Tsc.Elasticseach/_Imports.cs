// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Service.Caller;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Log;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model.Aggregate;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Service;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;
global using Masa.Contrib.Service.Caller;
global using Masa.Contrib.Service.Caller.HttpClient;
global using Masa.Contrib.StackSdks.Tsc.Elasticseach.Const;
global using Masa.Contrib.StackSdks.Tsc.Elasticseach.Converters;
global using Masa.Contrib.StackSdks.Tsc.Elasticseach.Model;
global using Masa.Utils.Data.Elasticsearch;
global using Microsoft.Extensions.DependencyInjection;
global using Nest;
global using System.Runtime.CompilerServices;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
