// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Response.Document;

public class ClearDocumentResponse : ResponseBase
{
    public ClearDocumentResponse(DeleteByQueryResponse response) : base(response)
    {
    }
}
