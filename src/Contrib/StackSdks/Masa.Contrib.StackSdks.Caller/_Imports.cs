// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using IdentityModel;
global using IdentityModel.Client;
global using Masa.BuildingBlocks.Service.Caller;
global using Masa.BuildingBlocks.StackSdks.Isolation;
global using Masa.BuildingBlocks.Isolation;
global using Masa.Contrib.Service.Caller.HttpClient;
global using Masa.Contrib.StackSdks.Caller;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Primitives;
global using Microsoft.IdentityModel.Tokens;
global using System.IdentityModel.Tokens.Jwt;
global using System.Net.Http.Headers;
global using System.Reflection;
global using System.Security.Claims;
global using System.Security.Cryptography;
