// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Net.Http;
using System.Text;

namespace Masa.Contrib.BasicAbility.Tsc.Tests;

internal class Common
{

    public static HttpRequestMessage CreateMessage(string url, object body)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (body != null)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        }
        return request;
    }
}
