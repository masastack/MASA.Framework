﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Options;

namespace Masa.Utils.Data.Elasticsearch;

public interface IElasticClientFactory
{
    IElasticClient Create();

    IElasticClient Create(string name);
}
