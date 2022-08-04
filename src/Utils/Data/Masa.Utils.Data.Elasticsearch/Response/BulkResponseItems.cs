// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Response;

public class BulkResponseItems
{
    /// <summary>
    /// The id of the document for the bulk operation
    /// </summary>
    public string Id { get; }

    public bool IsValid { get; }

    public string Message { get; }

    public BulkResponseItems(string id, bool isValid, string message)
    {
        Id = id;
        IsValid = isValid;
        Message = message;
    }
}
