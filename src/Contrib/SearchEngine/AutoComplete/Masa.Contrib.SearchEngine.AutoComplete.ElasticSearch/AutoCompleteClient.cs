// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch;

public class AutoCompleteClient : AutoCompleteClientBase
{
    private readonly IElasticClient _elasticClient;
    private readonly IMasaElasticClient _client;
    private readonly ILogger<AutoCompleteClient>? _logger;
    private readonly string? _alias;
    private readonly string _indexName;
    private readonly Operator _defaultOperator;
    private readonly SearchType _defaultSearchType;
    private readonly bool _enableMultipleCondition;
    private readonly string _queryAnalyzer;
    private readonly Action<IIndexSettings>? _indexSettingAction;
    private readonly Action<ITypeMapping>? _action;
    private readonly Type _documentType;
    private readonly ElasticsearchOptions _elasticsearchOptions;
    private readonly IElasticClientProvider? _elasticClientProvider;

#pragma warning disable S3011
    private static readonly MethodInfo AutoMapMethodInfo = typeof(AutoCompleteClient).GetMethod(nameof(AutoMap),
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)!;
#pragma warning restore S3011

    public AutoCompleteClient(
        IElasticClient elasticClient,
        IMasaElasticClient client,
        ILogger<AutoCompleteClient>? logger,
        ElasticSearchAutoCompleteOptions options,
        Type documentType)
    {
        _elasticClient = elasticClient;
        _client = client;
        _logger = logger;
        _indexName = options.IndexName;

        MasaArgumentException.ThrowIfNullOrWhiteSpace(options.IndexName);
        _alias = options.Alias;
        _defaultOperator = options.DefaultOperator;
        _defaultSearchType = options.DefaultSearchType;
        _enableMultipleCondition = options.EnableMultipleCondition;
        _queryAnalyzer = options.QueryAnalyzer;
        _indexSettingAction = options.IndexSettingAction;
        _action = options.Action;
        _documentType = documentType;
        _elasticsearchOptions = options.ElasticsearchOptions;
    }

    public AutoCompleteClient(
        IElasticClient elasticClient,
        IMasaElasticClient client,
        ILogger<AutoCompleteClient>? logger,
        ElasticSearchAutoCompleteOptions options,
        Type documentType,
        IElasticClientProvider elasticClientProvider) : this(elasticClient, client, logger, options, documentType)
    {
        _elasticClientProvider = elasticClientProvider;
    }

    public override async Task<bool> BuildAsync(CancellationToken cancellationToken = default)
    {
        var existsResponse = await _client.IndexExistAsync(_indexName, cancellationToken);
        if (!existsResponse.IsValid || existsResponse.Exists)
        {
            if (!existsResponse.IsValid)
                _logger?.LogError("AutoComplete: Initialization index is abnormal, {Message}", existsResponse.Message);

            return false;
        }

        IAnalysis analysis = new AnalysisDescriptor();
        analysis.Analyzers = new Analyzers();
        analysis.TokenFilters = new TokenFilters();
        IIndexSettings indexSettings = new IndexSettings()
        {
            Analysis = analysis
        };
        string analyzer = "ik_max_word_pinyin";
        if (_indexSettingAction != null)
            _indexSettingAction.Invoke(indexSettings);
        else
        {
            string pinyinFilter = "pinyin";
            indexSettings.Analysis.Analyzers.Add(analyzer, new CustomAnalyzer()
            {
                Filter = new[]
                {
                    pinyinFilter, "lowercase"
                },
                Tokenizer = "ik_max_word"
            });
            indexSettings.Analysis.TokenFilters.Add(pinyinFilter, new PinYinTokenFilterDescriptor());
        }

        var typeMappingDescriptorType = typeof(TypeMappingDescriptor<>).MakeGenericType(_documentType);
        var mapping = (ITypeMapping)Activator.CreateInstance(typeMappingDescriptorType)!;

        if (_action != null) _action.Invoke(mapping);
        else
        {
            mapping = (ITypeMapping)AutoMapMethodInfo.MakeGenericMethod(_documentType).Invoke(this, new object?[]
            {
                mapping, analyzer
            })!;
        }

        IAliases? aliases = null;
        if (_alias != null)
        {
            aliases = new Aliases();
            aliases.Add(_alias, new Alias());
        }
        var createIndexResponse = await _client.CreateIndexAsync(_indexName, new CreateIndexOptions()
        {
            Aliases = aliases,
            Mappings = mapping,
            IndexSettings = indexSettings
        }, cancellationToken).ConfigureAwait(false);
        if (!createIndexResponse.IsValid)
        {
            _logger?.LogWarning("Create index failed {Message}", createIndexResponse.Message);
        }
        return createIndexResponse.IsValid;
    }

    private static TypeMappingDescriptor<TDocument> AutoMap<TDocument>(
        TypeMappingDescriptor<TDocument> mapping,
        string analyzer) where TDocument : AutoCompleteDocument
    {
        mapping = mapping
            .AutoMap<TDocument>()
            .Properties(ps =>
                ps.Text(s =>
                    s.Name(n => n.Text)
                        .Analyzer(analyzer)
                )
            );
        return mapping;
    }

    public override async Task<bool> RebuildAsync(CancellationToken cancellationToken = default)
    {
        if (_alias != null) await _client.DeleteIndexByAliasAsync(_alias, cancellationToken);
        else await _client.DeleteIndexAsync(_indexName, cancellationToken);

        return await BuildAsync(cancellationToken);
    }

    public override async Task<Masa.BuildingBlocks.SearchEngine.AutoComplete.Response.GetResponse<TAudoCompleteDocument>>
        GetBySpecifyDocumentAsync<TAudoCompleteDocument>(
            string keyword,
            AutoCompleteOptions? options = null,
            CancellationToken cancellationToken = default)
    {
        var newOptions = options ?? new(_defaultSearchType);
        var searchType = newOptions.SearchType ?? _defaultSearchType;

        keyword = keyword.Trim();

        if (string.IsNullOrEmpty(keyword))
            return new Masa.BuildingBlocks.SearchEngine.AutoComplete.Response.GetResponse<TAudoCompleteDocument>(true, string.Empty,
                new List<TAudoCompleteDocument>());

        if (searchType == SearchType.Fuzzy)
        {
            var ret = await _client.GetPaginatedListAsync(
                new PaginatedOptions<TAudoCompleteDocument>(
                    _alias ?? _indexName,
                    GetFuzzyKeyword(keyword),
                    newOptions.Field,
                    newOptions.Page,
                    newOptions.PageSize,
                    _defaultOperator)
                {
                    Analyzer = _queryAnalyzer
                }
                , cancellationToken);
            return new Masa.BuildingBlocks.SearchEngine.AutoComplete.Response.GetResponse<TAudoCompleteDocument>(ret.IsValid, ret.Message)
            {
                Total = ret.Total,
                TotalPages = ret.TotalPages,
                Data = ret.Data
            };
        }
        else
        {
            var ret = await _elasticClient.SearchAsync<TAudoCompleteDocument>(s => s
                    .Index(_alias ?? _indexName)
                    .From((newOptions.Page - 1) * newOptions.PageSize)
                    .Size(newOptions.PageSize)
                    .Query(q => GetQueryDescriptor(q, newOptions.Field, keyword.ToLower()))
                , cancellationToken
            );
            return new Masa.BuildingBlocks.SearchEngine.AutoComplete.Response.GetResponse<TAudoCompleteDocument>(ret.IsValid,
                ret.ServerError?.ToString() ?? "")
            {
                Data = ret.Hits.Select(hit => hit.Source).ToList(),
                Total = ret.Total,
                TotalPages = (int)Math.Ceiling(ret.Total / (decimal)newOptions.PageSize)
            };
        }
    }

    private string GetFuzzyKeyword(string keyword)
    {
        if (_enableMultipleCondition)
            return string.Join(' ', keyword.Split(' ').Select(CompleteKeyword));

        if (!keyword.Contains(" "))
            return CompleteKeyword(keyword);

        return $"\"{keyword}\""; //Content contains spaces and is treated as a phrase for search
    }

    private string CompleteKeyword(string keyword)
    {
        if (keyword.Equals("*"))
            return keyword;

        keyword = keyword.Trim('*');
        return $"({keyword} OR *{keyword} OR {keyword}*)";
    }

    private QueryContainer GetQueryDescriptor<T>(QueryContainerDescriptor<T> queryContainerDescriptor, string field, string keyword)
        where T : class
    {
        var queryContainer = _defaultOperator == Operator.And ?
            queryContainerDescriptor.Bool(boolQueryDescriptor => GetBoolQueryDescriptor(boolQueryDescriptor, field, keyword)) :
            queryContainerDescriptor.Terms(descriptor
                => descriptor.Field(field).Terms(_enableMultipleCondition ? keyword.Split(' ') : new[]
                {
                    keyword
                }));
        return queryContainer;
    }

    private BoolQueryDescriptor<T> GetBoolQueryDescriptor<T>(BoolQueryDescriptor<T> boolQueryDescriptor, string field, string keyword)
        where T : class
    {
        if (!_enableMultipleCondition)
            return boolQueryDescriptor.Must(queryContainerDescriptor => queryContainerDescriptor.Term(field, keyword));

        foreach (var item in keyword.Split(' '))
        {
            boolQueryDescriptor = boolQueryDescriptor.Must(queryContainerDescriptor => queryContainerDescriptor.Term(field, item));
        }
        return boolQueryDescriptor;
    }


    public override Task<SetResponse> SetBySpecifyDocumentAsync<TAudoCompleteDocument>(IEnumerable<TAudoCompleteDocument> documents,
        SetOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        SetOptions newOptions = options ?? new();
        if (newOptions.IsOverride)
            return SetMultiAsync(documents, cancellationToken);

        return SetByNotOverrideAsync(documents, cancellationToken);
    }

    /// <summary>
    /// Set documents in batches
    /// add them if they don’t exist, update them if they exist
    /// </summary>
    /// <param name="documents"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TAudoCompleteDocument"></typeparam>
    /// <returns></returns>
    private async Task<SetResponse> SetMultiAsync<TAudoCompleteDocument>(
        IEnumerable<TAudoCompleteDocument> documents,
        CancellationToken cancellationToken = default)
        where TAudoCompleteDocument : AutoCompleteDocument
    {
        var request = new SetDocumentRequest<TAudoCompleteDocument>(_indexName);
        foreach (var document in documents)
            request.AddDocument(document, document.GetDocumentId());

        var ret = await _client.SetDocumentAsync(request, cancellationToken);
        return new SetResponse(ret.IsValid, ret.Message)
        {
            Items = ret.Items.Select(item => new SetResponseItems(item.Id, item.IsValid, item.Message)).ToList()
        };
    }

    /// <summary>
    /// Set documents in batches
    /// Update if it does not exist, skip if it exists
    /// </summary>
    /// <param name="documents"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TAudoCompleteDocument"></typeparam>
    /// <returns></returns>
    private async Task<SetResponse> SetByNotOverrideAsync<TAudoCompleteDocument>(
        IEnumerable<TAudoCompleteDocument> documents,
        CancellationToken cancellationToken = default)
        where TAudoCompleteDocument : AutoCompleteDocument
    {
        var request = new CreateMultiDocumentRequest<TAudoCompleteDocument>(_indexName);
        foreach (var document in documents)
            request.AddDocument(document, document.GetDocumentId());

        var ret = await _client.CreateMultiDocumentAsync(request, cancellationToken);
        return new SetResponse(ret.IsValid, ret.Message)
        {
            Items = ret.Items.Select(item => new SetResponseItems(item.Id, item.IsValid, item.Message)).ToList()
        };
    }

    public override async Task<Masa.BuildingBlocks.SearchEngine.AutoComplete.Response.DeleteResponse> DeleteAsync(string id,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.DeleteDocumentAsync(new DeleteDocumentRequest(_indexName, id), cancellationToken);
        return new Masa.BuildingBlocks.SearchEngine.AutoComplete.Response.DeleteResponse(response.IsValid, response.Message);
    }

    public override async Task<DeleteMultiResponse> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        var response = await _client.DeleteMultiDocumentAsync(new DeleteMultiDocumentRequest(_indexName, ids.ToArray()), cancellationToken);
        return new DeleteMultiResponse(response.IsValid, response.Message,
            response.Data.Select(item => new DeleteRangeResponseItems(item.Id, item.IsValid, item.Message)));
    }

    protected override void Dispose(bool disposing)
    {
        _elasticClientProvider?.TryRemove(_elasticsearchOptions);
        base.Dispose();
    }
}
