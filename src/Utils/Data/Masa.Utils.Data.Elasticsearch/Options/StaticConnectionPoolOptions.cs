// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options;

public class StaticConnectionPoolOptions
{
    public bool Randomize { get; set; }

    public IDateTimeProvider? DateTimeProvider { get; set; }

    public StaticConnectionPoolOptions()
    {
        Randomize = true;
        DateTimeProvider = null;
    }
}
