namespace Masa.Contrib.SearchEngine.AutoComplete;

public class AutoCompleteClient : IAutoCompleteClient
{
    private readonly IElasticClient _elasticClient;
    private readonly IMasaElasticClient _client;
    private readonly string _indexName;
    private readonly Operator _defaultOperator;
    private readonly SearchType _defaultSearchType;

    public AutoCompleteClient(IElasticClient elasticClient, IMasaElasticClient client, string indexName, Operator defaultOperator, SearchType defaultSearchType)
    {
        _elasticClient = elasticClient;
        _client = client;
        _indexName = indexName;
        _defaultOperator = defaultOperator;
        _defaultSearchType = defaultSearchType;
    }

    public Task<GetResponse<AutoCompleteDocument<TValue>, TValue>> GetAsync<TValue>(string value,
        AutoCompleteOptions? options = null,
        CancellationToken cancellationToken = default)
        => GetAsync<AutoCompleteDocument<TValue>, TValue>(value, options, cancellationToken);

    public async Task<GetResponse<TResponse, TValue>> GetAsync<TResponse, TValue>(string value,
        AutoCompleteOptions? options = null,
        CancellationToken cancellationToken = default)
        where TResponse : AutoCompleteDocument<TValue>
    {
        var newOptions = options ?? new (_defaultSearchType);
        if (newOptions.SearchType == SearchType.Fuzzy)
        {
            var ret = await _client.GetPaginatedListAsync(
                new PaginatedOptions<TResponse>(
                    _indexName,
                    value,
                    newOptions.Field,
                    newOptions.Page,
                    newOptions.PageSize,
                    _defaultOperator)
                , cancellationToken);
            return new GetResponse<TResponse, TValue>(ret.IsValid, ret.Message)
            {
                Total = ret.Total,
                TotalPages = ret.TotalPages,
                Data = ret.Data
            };
        }
        else
        {
            var ret = await _elasticClient.SearchAsync<TResponse>(s => s
                    .Index(_indexName)
                    .From((newOptions.Page - 1) * newOptions.PageSize)
                    .Size(newOptions.PageSize)
                    .Query(q => q
                        .Bool(b => b
                            .Must(queryContainerDescriptor => queryContainerDescriptor.Term(newOptions.Field, value))))
                , cancellationToken
            );
            return new GetResponse<TResponse, TValue>(ret.IsValid, ret.ServerError?.ToString() ?? "")
            {
                Data = ret.Hits.Select(hit => hit.Source).ToList(),
                Total = ret.Total,
                TotalPages = (int)Math.Ceiling(ret.Total / (decimal)newOptions.PageSize)
            };
        }
    }

    public Task<SetResponse> SetAsync<TValue>(
        AutoCompleteDocument<TValue>[] results,
        SetOptions? options = null,
        CancellationToken cancellationToken = default)
        => SetAsync<AutoCompleteDocument<TValue>, TValue>(results, options, cancellationToken);

    public Task<SetResponse> SetAsync<TDocument, TValue>(
        TDocument[] documents,
        SetOptions? options = null,
        CancellationToken cancellationToken = default) where TDocument : AutoCompleteDocument<TValue>
    {
        SetOptions newOptions = options ?? new();
        if (newOptions.IsOverride)
            return SetAsync<TDocument, TValue>(documents, cancellationToken);

        return SetByNotOverrideAsync<TDocument, TValue>(documents, cancellationToken);
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
    private async Task<SetResponse> SetAsync<TDocument, TValue>(
        TDocument[] documents,
        CancellationToken cancellationToken = default)
        where TDocument : AutoCompleteDocument<TValue>
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
        TDocument[] documents,
        CancellationToken cancellationToken = default) where TDocument : AutoCompleteDocument<TValue>
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
}
