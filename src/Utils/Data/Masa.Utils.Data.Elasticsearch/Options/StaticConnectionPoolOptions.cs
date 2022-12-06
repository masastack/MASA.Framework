// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options;

public class StaticConnectionPoolOptions
{
    internal bool Randomize { get; set; }

    internal IDateTimeProvider? DateTimeProvider { get; set; }

    public StaticConnectionPoolOptions()
    {
        Randomize = true;
        DateTimeProvider = null;
    }

    internal StaticConnectionPoolOptions UseRandomize(bool randomize)
    {
        Randomize = randomize;
        return this;
    }

    internal StaticConnectionPoolOptions UseDateTimeProvider(IDateTimeProvider? dateTimeProvider)
    {
        DateTimeProvider = dateTimeProvider;
        return this;
    }
}
