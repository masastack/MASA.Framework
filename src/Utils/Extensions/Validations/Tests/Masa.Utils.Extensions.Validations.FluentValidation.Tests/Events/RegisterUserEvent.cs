// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

public class RegisterUserEvent
{
    public string? Name { get; set; }

    public string? IdCard { get; set; }

    public string? Phone { get; set; }

    public string? Port { get; set; }

    public string? Referer { get; set; }

    public string? Remark { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }
}
