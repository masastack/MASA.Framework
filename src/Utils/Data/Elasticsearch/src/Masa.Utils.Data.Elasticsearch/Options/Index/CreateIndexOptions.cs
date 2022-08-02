// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Index;

public class CreateIndexOptions
{
    public IIndexSettings? IndexSettings { get; set; } = null;

    public IAliases? Aliases { get; set; } = null;

    public ITypeMapping? Mappings { get; set; } = null;
}
