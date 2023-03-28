// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete;

internal class DefaultIAutoCompleteClient : IManualAutoCompleteClient
{
    private readonly IManualAutoCompleteClient _autoCompleteClient;

    public DefaultIAutoCompleteClient(IManualAutoCompleteClient autoCompleteClient)
        => _autoCompleteClient = autoCompleteClient;

    public Task<bool> BuildAsync(CancellationToken cancellationToken = default)
        => _autoCompleteClient.BuildAsync(cancellationToken);

    public Task RebuildAsync(CancellationToken cancellationToken = default)
        => _autoCompleteClient.RebuildAsync(cancellationToken);

    public Task<GetResponse<AutoCompleteDocument<Guid>>> GetAsync(
        string keyword,
        AutoCompleteOptions? options = null,
        CancellationToken cancellationToken = default)
        => _autoCompleteClient.GetAsync(keyword, options, cancellationToken);

    public Task<GetResponse<AutoCompleteDocument<TValue>>> GetAsync<TValue>(
        string keyword,
        AutoCompleteOptions? options = null,
        CancellationToken cancellationToken = default) where TValue : notnull
        => _autoCompleteClient.GetAsync<TValue>(keyword, options, cancellationToken);

    [Obsolete($"{nameof(GetAsync)} expired, please use {nameof(GetBySpecifyDocumentAsync)}")]
    public Task<GetResponse<TAudoCompleteDocument>> GetAsync<TAudoCompleteDocument, TValue>(
        string keyword,
        AutoCompleteOptions? options = null,
        CancellationToken cancellationToken = default) where TAudoCompleteDocument : AutoCompleteDocument
        => _autoCompleteClient.GetAsync<TAudoCompleteDocument, TValue>(keyword, options, cancellationToken);

    public Task<GetResponse<TAudoCompleteDocument>> GetBySpecifyDocumentAsync<TAudoCompleteDocument>(
        string keyword,
        AutoCompleteOptions? options = null,
        CancellationToken cancellationToken = default) where TAudoCompleteDocument : AutoCompleteDocument
        => _autoCompleteClient.GetBySpecifyDocumentAsync<TAudoCompleteDocument>(keyword, options, cancellationToken);

    public Task<SetResponse> SetAsync(
        AutoCompleteDocument<Guid> document,
        SetOptions? options = null,
        CancellationToken cancellationToken = default)
        => _autoCompleteClient.SetAsync(document, options, cancellationToken);

    public Task<SetResponse> SetAsync(
        IEnumerable<AutoCompleteDocument<Guid>> documents,
        SetOptions? options = null,
        CancellationToken cancellationToken = default)
        => _autoCompleteClient.SetAsync(documents, options, cancellationToken);

    public Task<SetResponse> SetAsync<TValue>(
        AutoCompleteDocument<TValue> document,
        SetOptions? options = null,
        CancellationToken cancellationToken = default) where TValue : notnull
        => _autoCompleteClient.SetAsync(document, options, cancellationToken);

    public Task<SetResponse> SetAsync<TValue>(
        IEnumerable<AutoCompleteDocument<TValue>> documents,
        SetOptions? options = null,
        CancellationToken cancellationToken = default) where TValue : notnull
        => _autoCompleteClient.SetAsync(documents, options, cancellationToken);

    [Obsolete($"{nameof(SetAsync)} expired, please use {nameof(SetBySpecifyDocumentAsync)}")]
    public Task<SetResponse> SetAsync<TAudoCompleteDocument, TValue>(
        TAudoCompleteDocument document,
        SetOptions? options = null,
        CancellationToken cancellationToken = default) where TAudoCompleteDocument : AutoCompleteDocument
        => _autoCompleteClient.SetAsync<TAudoCompleteDocument, TValue>(document, options, cancellationToken);

    [Obsolete($"{nameof(SetAsync)} expired, please use {nameof(SetBySpecifyDocumentAsync)}")]
    public Task<SetResponse> SetAsync<TAudoCompleteDocument, TValue>(
        IEnumerable<TAudoCompleteDocument> documents,
        SetOptions? options = null,
        CancellationToken cancellationToken = default) where TAudoCompleteDocument : AutoCompleteDocument
        => _autoCompleteClient.SetAsync<TAudoCompleteDocument, TValue>(documents, options, cancellationToken);

    public Task<SetResponse> SetBySpecifyDocumentAsync<TAudoCompleteDocument>(
        TAudoCompleteDocument document,
        SetOptions? options = null,
        CancellationToken cancellationToken = default) where TAudoCompleteDocument : AutoCompleteDocument
        => _autoCompleteClient.SetBySpecifyDocumentAsync(document, options, cancellationToken);

    public Task<SetResponse> SetBySpecifyDocumentAsync<TAudoCompleteDocument>(
        IEnumerable<TAudoCompleteDocument> documents,
        SetOptions? options = null,
        CancellationToken cancellationToken = default) where TAudoCompleteDocument : AutoCompleteDocument
        => _autoCompleteClient.SetBySpecifyDocumentAsync<TAudoCompleteDocument>(documents, options, cancellationToken);

    public Task<DeleteResponse> DeleteAsync(string id, CancellationToken cancellationToken = default)
        => _autoCompleteClient.DeleteAsync(id, cancellationToken);

    public Task<DeleteResponse> DeleteAsync<T>(T id, CancellationToken cancellationToken = default) where T : IComparable
        => _autoCompleteClient.DeleteAsync(id, cancellationToken);

    public Task<DeleteMultiResponse> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        => _autoCompleteClient.DeleteAsync(ids, cancellationToken);

    public Task<DeleteMultiResponse> DeleteAsync<T>(IEnumerable<T> ids, CancellationToken cancellationToken = default) where T : IComparable
        => _autoCompleteClient.DeleteAsync(ids, cancellationToken);

    public void Dispose()
    {
        //don't need to be released
    }
}
