namespace Masa.Contrib.SearchEngine.AutoComplete;

public class AutoCompleteClient : BaseAutoCompleteClient
{
    private readonly IElasticClient _elasticClient;
    private readonly IMasaElasticClient _client;
    private readonly string _indexName;
    private readonly Operator _defaultOperator;
    private readonly SearchType _defaultSearchType;

    public AutoCompleteClient(IElasticClient elasticClient, IMasaElasticClient client, string indexName, Operator defaultOperator,
        SearchType defaultSearchType)
    {
        _elasticClient = elasticClient;
        _client = client;
        _indexName = indexName;
        _defaultOperator = defaultOperator;
        _defaultSearchType = defaultSearchType;
    }

    public override async Task<GetResponse<TAudoCompleteDocument, TValue>> GetAsync<TAudoCompleteDocument, TValue>(string keyword,
        AutoCompleteOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var newOptions = options ?? new(_defaultSearchType);
        var searchType = newOptions.SearchType ?? _defaultSearchType;
        if (searchType == SearchType.Fuzzy)
        {
            var ret = await _client.GetPaginatedListAsync(
                new PaginatedOptions<TAudoCompleteDocument>(
                    _indexName,
                    keyword,
                    newOptions.Field,
                    newOptions.Page,
                    newOptions.PageSize,
                    _defaultOperator)
                , cancellationToken);
            return new GetResponse<TAudoCompleteDocument, TValue>(ret.IsValid, ret.Message)
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
                    .Query(q => q
                        .Bool(b => b
                            .Must(queryContainerDescriptor => queryContainerDescriptor.Term(newOptions.Field, keyword))))
                , cancellationToken
            );
            return new GetResponse<TAudoCompleteDocument, TValue>(ret.IsValid, ret.ServerError?.ToString() ?? "")
            {
                Data = ret.Hits.Select(hit => hit.Source).ToList(),
                Total = ret.Total,
                TotalPages = (int)Math.Ceiling(ret.Total / (decimal)newOptions.PageSize)
            };
        }
    }

    public override Task<SetResponse> SetMultiAsync<TAudoCompleteDocument, TValue>(IEnumerable<TAudoCompleteDocument> documents,
        SetOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        SetOptions newOptions = options ?? new();
        if (newOptions.IsOverride)
            return SetMultiAsync<TAudoCompleteDocument, TValue>(documents, cancellationToken);

        return SetByNotOverrideAsync<TAudoCompleteDocument, TValue>(documents, cancellationToken);
    }

    /// <summary>
    /// Set documents in batches
    /// add them if they don’t exist, update them if they exist
    /// </summary>
    /// <param name="documents"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TDocument"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    private async Task<SetResponse> SetMultiAsync<TDocument, TValue>(
        IEnumerable<TDocument> documents,
        CancellationToken cancellationToken = default)
        where TDocument : AutoCompleteDocument<TValue> where TValue : notnull
    {
        var request = new SetDocumentRequest<TDocument>(_indexName);
        foreach (var document in documents)
            request.AddDocument(document, document.Id);

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
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    private async Task<SetResponse> SetByNotOverrideAsync<TDocument, TValue>(
        IEnumerable<TDocument> documents,
        CancellationToken cancellationToken = default)
        where TDocument : AutoCompleteDocument<TValue> where TValue : notnull
    {
        var request = new CreateMultiDocumentRequest<TDocument>(_indexName);
        foreach (var document in documents)
            request.AddDocument(document, document.Id);

        var ret = await _client.CreateMultiDocumentAsync(request, cancellationToken);
        return new SetResponse(ret.IsValid, ret.Message)
        {
            Items = ret.Items.Select(item => new SetResponseItems(item.Id, item.IsValid, item.Message)).ToList()
        };
    }

    public override async Task<DeleteResponse> DeleteAsync(string id)
    {
        var response = await _client.DeleteDocumentAsync(new DeleteDocumentRequest(_indexName, id));
        return new DeleteResponse(response.IsValid, response.Message);
    }

    public override async Task<DeleteMultiResponse> DeleteMultiAsync(IEnumerable<string> ids)
    {
        var response = await _client.DeleteMultiDocumentAsync(new DeleteMultiDocumentRequest(_indexName, ids.ToArray()));
        return new DeleteMultiResponse(response.IsValid, response.Message,
            response.Data.Select(item => new DeleteRangeResponseItems(item.Id, item.IsValid, item.Message)));
    }
}
