// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.StackSdks.Tsc.Tests")]
namespace Masa.Contrib.Service.Caller;

internal static class CallerProviderExtensions
{
    public static async Task<TResult> GetByBodyAsync<TResult>(this ICaller caller, string url, object? body) where TResult : class
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (body is not null)
        {
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        }
        return (await caller.SendAsync<TResult>(request))!;
    }
}
