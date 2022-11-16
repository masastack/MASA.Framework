// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model;

public class FieldConditionDto
{
    public string Name { get; set; }

    public ConditionTypes Type { get; set; }

    public object Value { get; set; }
}
