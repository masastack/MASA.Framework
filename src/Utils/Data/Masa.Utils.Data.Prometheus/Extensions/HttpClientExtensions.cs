// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Utils.Data.Prometheus.Test")]
namespace System.Net.Http;

internal static class HttpClientExtensions
{
    public static async Task<string> GetAsync(this HttpClient client, string url, object data, ILogger logger)
    {
        try
        {
            var rep = await client.GetAsync($"{url}?{data.ToUrlParam()}");
            var str = await rep.Content.ReadAsStringAsync();
            if (rep.IsSuccessStatusCode)
            {
                return str;
            }
            else
            {
                return str ?? $"{{\"status\":\"error\",\"errorType\":\"unkown\",\"error\":\"{(int)rep.StatusCode}-{rep.StatusCode}\"}}";
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Request Exception methd: GET, url:{url} ,data:{data}", url, data);
            return $"{{\"status\":\"error\",\"errorType\":\"unkown\",\"error\":\"{ex.Message}\"}}";
        }
    }
}
