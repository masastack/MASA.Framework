// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Count;

public class CountDocumentRequest : DocumentOptions
{
    public CountDocumentRequest(string indexNameOrAlias) : base(indexNameOrAlias)
    {
    }
}
