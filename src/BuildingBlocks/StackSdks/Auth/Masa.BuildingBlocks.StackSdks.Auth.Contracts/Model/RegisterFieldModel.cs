// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class RegisterFieldModel
{
    public RegisterFieldTypes RegisterFieldType { get; private set; }

    public int Sort { get; private set; }

    public bool Required { get; private set; }
}
