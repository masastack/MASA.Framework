// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public class ValidationModel
{
    public string Name { get; set; }

    public string Message { get; set; }

    public ValidationLevel Level { get; set; }

    public ValidationModel(ValidationLevel level = ValidationLevel.Error)
    {
        Level = level;
    }

    public ValidationModel(string name, string message, ValidationLevel level = ValidationLevel.Error)
        : this(level)
    {
        Name = name;
        Message = message;
    }
}
