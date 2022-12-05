// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Utils.Models;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ResponseBase
{
    public bool IsValid { get; }

    public string Message { get; }

    public ResponseBase(bool isValid, string message = "")
    {
        IsValid = isValid;
        Message = message;
    }
}
