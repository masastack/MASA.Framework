// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.Tests.Scenes.NoLessConstructor;

public class EsOptions : LocalMasaConfigurationOptions
{
    public string[] Nodes { get; set; }

    public EsOptions(string[] nodes)
    {
        Nodes = nodes;
    }
}
