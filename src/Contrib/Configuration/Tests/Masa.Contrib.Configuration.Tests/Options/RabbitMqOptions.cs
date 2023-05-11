// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration.Tests;

public class RabbitMqOptions : LocalMasaOptionsConfigurable
{
    public string HostName { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string VirtualHost { get; set; }

    public string Port { get; set; }

    /// <summary>
    /// The section null means same as the class name, else load from the specify section
    /// </summary>
    [JsonIgnore]
    public override string? Section => "RabbitMq";
}
