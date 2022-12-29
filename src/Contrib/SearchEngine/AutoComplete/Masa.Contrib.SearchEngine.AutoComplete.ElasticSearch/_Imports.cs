// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.SearchEngine.AutoComplete;
global using Masa.BuildingBlocks.SearchEngine.AutoComplete.Options;
global using Masa.BuildingBlocks.SearchEngine.AutoComplete.Response;
global using Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch;
global using Masa.Utils.Data.Elasticsearch;
global using Masa.Utils.Data.Elasticsearch.Analysis.TokenFilters;
global using Masa.Utils.Data.Elasticsearch.Options.Document.Create;
global using Masa.Utils.Data.Elasticsearch.Options.Document.Delete;
global using Masa.Utils.Data.Elasticsearch.Options.Document.Query;
global using Masa.Utils.Data.Elasticsearch.Options.Document.Set;
global using Masa.Utils.Data.Elasticsearch.Options.Index;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Nest;
