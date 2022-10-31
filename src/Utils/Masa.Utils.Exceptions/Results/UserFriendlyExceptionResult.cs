// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.AspNetCore.Mvc;

public class UserFriendlyExceptionResult : IActionResult
{
    public string Message { get; set; }

    public UserFriendlyExceptionResult(string message)
    {
        Message = message;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        await context.HttpContext.Response.WriteTextAsync((int)MasaHttpStatusCode.UserFriendlyException, Message);
    }
}
