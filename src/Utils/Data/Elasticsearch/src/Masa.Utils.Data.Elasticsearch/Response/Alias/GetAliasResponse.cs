// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Response.Alias;

public class GetAliasResponse : ResponseBase
{
    public IEnumerable<string> Aliases { get; }

    public GetAliasResponse(CatResponse<CatAliasesRecord> catResponse) : base(catResponse)
        => Aliases = catResponse.IsValid ? catResponse.Records.Select(r => r.Alias).ToArray() : Array.Empty<string>();

    public GetAliasResponse(Nest.GetAliasResponse getAliasResponse) : base(getAliasResponse)
    {
        Aliases = getAliasResponse.IsValid
            ? getAliasResponse.Indices
                .Select(item => item.Value)
                .SelectMany(indexAlias => indexAlias.Aliases)
                .Select(alias => alias.Key).Distinct().ToArray()
            : Array.Empty<string>();
    }
}
