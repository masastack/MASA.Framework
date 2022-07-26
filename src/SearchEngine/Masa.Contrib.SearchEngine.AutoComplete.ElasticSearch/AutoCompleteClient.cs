// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.SearchEngine.AutoComplete;

public class AutoCompleteClient : BaseAutoCompleteClient
{
    private readonly IElasticClient _elasticClient;
    private readonly IMasaElasticClient _client;
    private readonly string _indexName;
    private readonly Operator _defaultOperator;
    private readonly SearchType _defaultSearchType;
    private readonly bool _enableMultipleCondition;

    public AutoCompleteClient(
        IElasticClient elasticClient,
        IMasaElasticClient client,
        string indexName,
        Operator defaultOperator,
        SearchType defaultSearchType,
        bool enableMultipleCondition)
    {
        _elasticClient = elasticClient;
        _client = client;
        _indexName = indexName;
        _defaultOperator = defaultOperator;
        _defaultSearchType = defaultSearchType;
        _enableMultipleCondition = enableMultipleCondition;
    }

    public override async Task<Masa.BuildingBlocks.SearchEngine.AutoComplete.Response.GetResponse<TAudoCompleteDocument>> GetBySpecifyDocumentAsync<TAudoCompleteDocument>(
        string keyword,
        AutoCompleteOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var newOptions = options ?? new(_defaultSearchType);
        var searchType = newOptions.SearchType ?? _defaultSearchType;

        keyword = keyword.Trim();

        if (string.IsNullOrEmpty(keyword))
            return new Masa.BuildingBlocks.SearchEngine.AutoComplete.Response.GetResponse<TAudoCompleteDocument>(true, string.Empty, new List<TAudoCompleteDocument>());

        if (searchType == SearchType.Fuzzy)
        {
            var ret = await _client.GetPaginatedListAsync(
                new PaginatedOptions<TAudoCompleteDocument>(
                    _indexName,
                    GetFuzzyKeyword(keyword),
                    newOptions.Field,
                    newOptions.Page,
                    newOptions.PageSize,
                    _defaultOperator)
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
                    .Index(_indexName)
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
                => descriptor.Field(field).Terms(_enableMultipleCondition ? keyword.Split(' ') : new[] { keyword }));
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
    /// <typeparam name="TDocument"></typeparam>
    /// <returns></returns>
    private async Task<SetResponse> SetMultiAsync<TDocument>(
        IEnumerable<TDocument> documents,
        CancellationToken cancellationToken = default)
        where TDocument : AutoCompleteDocument
    {
        var request = new SetDocumentRequest<TDocument>(_indexName);
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
    /// <typeparam name="TDocument"></typeparam>
    /// <returns></returns>
    private async Task<SetResponse> SetByNotOverrideAsync<TDocument>(
        IEnumerable<TDocument> documents,
        CancellationToken cancellationToken = default)
        where TDocument : AutoCompleteDocument
    {
        var request = new CreateMultiDocumentRequest<TDocument>(_indexName);
        foreach (var document in documents)
            request.AddDocument(document, document.GetDocumentId());

        var ret = await _client.CreateMultiDocumentAsync(request, cancellationToken);
        return new SetResponse(ret.IsValid, ret.Message)
        {
            Items = ret.Items.Select(item => new SetResponseItems(item.Id, item.IsValid, item.Message)).ToList()
        };
    }

    public override async Task<DeleteResponse> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _client.DeleteDocumentAsync(new DeleteDocumentRequest(_indexName, id), cancellationToken);
        return new DeleteResponse(response.IsValid, response.Message);
    }

    public override async Task<DeleteMultiResponse> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        var response = await _client.DeleteMultiDocumentAsync(new DeleteMultiDocumentRequest(_indexName, ids.ToArray()), cancellationToken);
        return new DeleteMultiResponse(response.IsValid, response.Message,
            response.Data.Select(item => new DeleteRangeResponseItems(item.Id, item.IsValid, item.Message)));
    }
}
