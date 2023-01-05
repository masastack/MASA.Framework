// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete.Tests;

public class CustomAutoCompleteClient : AutoCompleteClientBase
{
    public override Task<bool> BuildAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task RebuildAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<GetResponse<TAudoCompleteDocument>> GetBySpecifyDocumentAsync<TAudoCompleteDocument>(string keyword, AutoCompleteOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<SetResponse> SetBySpecifyDocumentAsync<TAudoCompleteDocument>(IEnumerable<TAudoCompleteDocument> documents, SetOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<DeleteResponse> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<DeleteMultiResponse> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        => Task.FromResult(new DeleteMultiResponse(true, ""));
}
