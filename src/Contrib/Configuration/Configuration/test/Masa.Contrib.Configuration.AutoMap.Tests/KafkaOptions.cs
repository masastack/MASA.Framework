// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.AutoMap.Tests;

public class KafkaOptions : LocalMasaConfigurationOptions
{
    public string Servers { get; set; }

    public int ConnectionPoolSize { get; set; }
}
