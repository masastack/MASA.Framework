// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Google.Protobuf;
global using Masa.Utils.Caller.Core;
global using Masa.Utils.Caller.Core.Internal;
global using Masa.Utils.Caller.Core.Internal.Options;
global using Masa.Utils.Exceptions;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using System.Collections.Concurrent;
global using System.Net;
global using System.Net.Http.Json;
global using System.Reflection;
global using System.Runtime.ExceptionServices;
global using System.Text.Json;
global using System.Text.Json.Serialization;
