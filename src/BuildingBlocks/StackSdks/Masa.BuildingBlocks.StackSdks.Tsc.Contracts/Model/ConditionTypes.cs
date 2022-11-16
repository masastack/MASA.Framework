// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model;

public enum ConditionTypes
{
    Equal=1,
    NotEqual,
    Great,
    Less,
    GreatEqual,
    LessEqual,
    In,
    NotIn,
    Regex,
    NotRegex,
    Exists,
    NotExists
}
