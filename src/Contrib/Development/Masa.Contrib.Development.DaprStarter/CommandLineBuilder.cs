// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter;

public class CommandLineBuilder
{
    public string Prefix { get; }

    public List<string> Arguments { get; set; }

    public CommandLineBuilder(string prefix)
    {
        Prefix = prefix;
        Arguments = new();
    }

    public CommandLineBuilder Add(string name, string value, bool isSkip = false)
    {
        if (!isSkip)
        {
            Arguments.Add($"{Prefix}{name} {value}");
        }

        return this;
    }

    public override string ToString() => string.Join(' ', Arguments);
}
