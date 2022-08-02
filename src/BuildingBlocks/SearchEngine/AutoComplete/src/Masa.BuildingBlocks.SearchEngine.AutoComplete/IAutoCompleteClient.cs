// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete;

public interface IAutoCompleteClient
{
    Task<GetResponse<AutoCompleteDocument<Guid>>> GetAsync(
        string keyword,
        AutoCompleteOptions? options = null,
        CancellationToken cancellationToken = default);

    Task<GetResponse<AutoCompleteDocument<TValue>>> GetAsync<TValue>(
        string keyword,
        AutoCompleteOptions? options = null,
        CancellationToken cancellationToken = default) where TValue : notnull;

    [Obsolete($"{nameof(GetAsync)} expired, please use {nameof(GetBySpecifyDocumentAsync)}")]
    Task<GetResponse<TAudoCompleteDocument>> GetAsync<TAudoCompleteDocument, TValue>(
        string keyword,
        AutoCompleteOptions? options = null,
        CancellationToken cancellationToken = default)
        where TAudoCompleteDocument : AutoCompleteDocument;

    Task<GetResponse<TAudoCompleteDocument>> GetBySpecifyDocumentAsync<TAudoCompleteDocument>(
        string keyword,
        AutoCompleteOptions? options = null,
        CancellationToken cancellationToken = default)
        where TAudoCompleteDocument : AutoCompleteDocument;

    Task<SetResponse> SetAsync(
        AutoCompleteDocument<Guid> document,
        SetOptions? options = null,
        CancellationToken cancellationToken = default);

    Task<SetResponse> SetAsync(
        IEnumerable<AutoCompleteDocument<Guid>> documents,
        SetOptions? options = null,
        CancellationToken cancellationToken = default);

    Task<SetResponse> SetAsync<TValue>(
        AutoCompleteDocument<TValue> document,
        SetOptions? options = null,
        CancellationToken cancellationToken = default) where TValue : notnull;

    Task<SetResponse> SetAsync<TValue>(
        IEnumerable<AutoCompleteDocument<TValue>> documents,
        SetOptions? options = null,
        CancellationToken cancellationToken = default) where TValue : notnull;

    [Obsolete($"{nameof(SetAsync)} expired, please use {nameof(SetBySpecifyDocumentAsync)}")]
    Task<SetResponse> SetAsync<TAudoCompleteDocument, TValue>(
        TAudoCompleteDocument document,
        SetOptions? options = null,
        CancellationToken cancellationToken = default) where TAudoCompleteDocument : AutoCompleteDocument;

    [Obsolete($"{nameof(SetAsync)} expired, please use {nameof(SetBySpecifyDocumentAsync)}")]
    Task<SetResponse> SetAsync<TAudoCompleteDocument, TValue>(
        IEnumerable<TAudoCompleteDocument> documents,
        SetOptions? options = null,
        CancellationToken cancellationToken = default) where TAudoCompleteDocument : AutoCompleteDocument;

    Task<SetResponse> SetBySpecifyDocumentAsync<TAudoCompleteDocument>(
        TAudoCompleteDocument document,
        SetOptions? options = null,
        CancellationToken cancellationToken = default) where TAudoCompleteDocument : AutoCompleteDocument;

    Task<SetResponse> SetBySpecifyDocumentAsync<TAudoCompleteDocument>(
        IEnumerable<TAudoCompleteDocument> documents,
        SetOptions? options = null,
        CancellationToken cancellationToken = default) where TAudoCompleteDocument : AutoCompleteDocument;

    Task<DeleteResponse> DeleteAsync(string id, CancellationToken cancellationToken = default);

    Task<DeleteResponse> DeleteAsync<T>(T id, CancellationToken cancellationToken = default) where T : IComparable;

    Task<DeleteMultiResponse> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

    Task<DeleteMultiResponse> DeleteAsync<T>(IEnumerable<T> ids, CancellationToken cancellationToken = default) where T : IComparable;
}
