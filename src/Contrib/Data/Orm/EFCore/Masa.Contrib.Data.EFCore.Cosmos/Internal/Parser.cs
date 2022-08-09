// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Internal;

internal static class Parser
{
    public static Dictionary<string, string> ToDictionary(this string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("Cosmos: empty database connection string", nameof(connectionString));

        Dictionary<string, string> dictionary = new();
        foreach (var item in connectionString.Split(';'))
        {
            if (string.IsNullOrEmpty(item))
                continue;

            if (item.Split('=').Length != 2)
                throw new ArgumentException("Cosmos: Bad database connection string");

            dictionary.Add(item.Split('=')[0], item.Split('=')[1]);
        }
        return dictionary;
    }
}
