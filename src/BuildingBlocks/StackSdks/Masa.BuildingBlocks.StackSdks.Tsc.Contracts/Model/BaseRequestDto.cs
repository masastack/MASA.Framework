// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model;

public class BaseRequestDto : PaginationRequestDto
{
    public string Keyword { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public string RawQuery { get; set; }

    public IEnumerable<FieldConditionDto> Conditions { get; set; }

    public IEnumerable<FieldOrderDto> Sorts { get; set; }

    public virtual void AppendConditions() { }
}
