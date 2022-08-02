// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Response;

public class ResponseBase
{
    public bool IsValid { get; }

    public string Message { get; }

    protected ResponseBase(bool isValid, string message)
    {
        IsValid = isValid;
        Message = message;
    }

    public ResponseBase(IResponse response) : this(response.IsValid, response.IsValid ? "success" : response.ServerError?.ToString() ?? string.Empty)
    {
    }
}
