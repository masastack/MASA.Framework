namespace MASA.BuildingBlocks.SearchEngine.AutoComplete;
public interface IAutoCompleteClient
{
    Task<GetResponse<AutoCompleteDocument<TValue>, TValue>> GetAsync<TValue>(
        string keyword,
        AutoCompleteOptions? options = null,
        CancellationToken cancellationToken = default);

    Task<GetResponse<TAudoCompleteDocument, TValue>> GetAsync<TAudoCompleteDocument, TValue>(
        string keyword,
        AutoCompleteOptions? options = null,
        CancellationToken cancellationToken = default)
        where TAudoCompleteDocument : AutoCompleteDocument<TValue>;

    Task<SetResponse> SetAsync<TValue>(
        AutoCompleteDocument<TValue>[] results,
        SetOptions? options = null,
        CancellationToken cancellationToken = default);

    Task<SetResponse> SetAsync<TAudoCompleteDocument, TValue>(
        TAudoCompleteDocument[] documents,
        SetOptions? options = null,
        CancellationToken cancellationToken = default) where TAudoCompleteDocument : AutoCompleteDocument<TValue> ;
}
