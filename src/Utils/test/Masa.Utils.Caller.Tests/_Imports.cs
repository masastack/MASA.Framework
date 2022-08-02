// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.Utils.Caller.Core;
global using Masa.Utils.Caller.HttpClient;
global using Masa.Utils.Exceptions;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Moq;
global using Moq.Protected;
global using System.Net;
global using System.Runtime.ExceptionServices;
global using System.Text;
global using System.Text.Json.Serialization;
global using System.Xml.Serialization;
global using System.Net.Http.Json;
global using Masa.Utils.Caller.Tests.Application;
global using Masa.Utils.Caller.Tests.Application.Requesties;
global using Masa.Utils.Caller.Tests.Application.Response;
global using Masa.Utils.Caller.Tests.Infrastructure;
global using Masa.Utils.Caller.Tests.Infrastructure.Utils;
global using Masa.Utils.Caller.Tests.Application.Queries;
global using Masa.Utils.Caller.DaprClient;
global using Masa.Utils.Caller.Tests.Infrastructure.Callers;
