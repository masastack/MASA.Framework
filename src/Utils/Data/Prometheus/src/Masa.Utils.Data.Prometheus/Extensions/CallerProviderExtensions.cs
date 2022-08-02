// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Utils.Data.Prometheus.Test")]
namespace Masa.Utils.Caller.Core;

internal static class CallerProviderExtensions
{
    public static async Task<string> GetAsync(this ICallerProvider caller, string url, object data)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{url}?{data.ToUrlParam()}");
        var response = await caller.SendAsync(request, autoThrowUserFriendlyException: false);
        return await response.Content.ReadAsStringAsync();
    }
}
