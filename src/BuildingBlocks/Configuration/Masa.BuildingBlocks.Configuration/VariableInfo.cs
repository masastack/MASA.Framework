// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration;

public class VariableInfo
{
    public string Variable { get; set; }

    public string DefaultValue { get; set; }

    public VariableInfo(string variable, string defaultValue)
    {
        Variable = variable;
        DefaultValue = defaultValue;
    }
}
