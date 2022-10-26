// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.RulesEngine;

public class VerifyResponse : ResponseBase
{
    public VerifyResponse(bool isValid, string message = "")
        : base(isValid, message)
    {
    }
}
