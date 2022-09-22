// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class RegisterFieldModel
{
    public RegisterFieldTypes RegisterFieldType { get; set; }

    public int Sort { get; set; }

    public bool Required { get; set; }
}
