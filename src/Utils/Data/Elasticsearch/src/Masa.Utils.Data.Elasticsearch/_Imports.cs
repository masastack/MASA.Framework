// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Elasticsearch.Net;
global using Masa.Utils.Data.Elasticsearch;
global using Masa.Utils.Data.Elasticsearch.Internal.BulkOperation;
global using Masa.Utils.Data.Elasticsearch.Options;
global using Masa.Utils.Data.Elasticsearch.Options.Alias;
global using Masa.Utils.Data.Elasticsearch.Options.Document.Count;
global using Masa.Utils.Data.Elasticsearch.Options.Document.Create;
global using Masa.Utils.Data.Elasticsearch.Options.Document.Delete;
global using Masa.Utils.Data.Elasticsearch.Options.Document.Exist;
global using Masa.Utils.Data.Elasticsearch.Options.Document.Get;
global using Masa.Utils.Data.Elasticsearch.Options.Document.Query;
global using Masa.Utils.Data.Elasticsearch.Options.Document.Set;
global using Masa.Utils.Data.Elasticsearch.Options.Document.Update;
global using Masa.Utils.Data.Elasticsearch.Options.Index;
global using Masa.Utils.Data.Elasticsearch.Response;
global using Masa.Utils.Data.Elasticsearch.Response.Document;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Nest;
global using System.Collections.Concurrent;
global using System.Runtime.Serialization;
global using MASABulkAliasResponse = Masa.Utils.Data.Elasticsearch.Response.Alias.BulkAliasResponse;
global using MASAGetAliasResponse = Masa.Utils.Data.Elasticsearch.Response.Alias.GetAliasResponse;
