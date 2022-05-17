// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Aliyun.Acs.Core;
global using Aliyun.Acs.Core.Auth.Sts;
global using Aliyun.Acs.Core.Profile;
global using Aliyun.OSS;
global using Aliyun.OSS.Util;
global using Masa.BuildingBlocks.Configuration;
global using Masa.BuildingBlocks.Storage.ObjectStorage;
global using Masa.BuildingBlocks.Storage.ObjectStorage.Response;
global using Masa.Contrib.Storage.ObjectStorage.Aliyun.Internal;
global using Masa.Contrib.Storage.ObjectStorage.Aliyun.Internal.Response;
global using Masa.Contrib.Storage.ObjectStorage.Aliyun.Options;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using System.Net;
global using System.Text;
global using AliyunFormatType = Aliyun.Acs.Core.Http.FormatType;
