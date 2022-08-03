// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Models;

public class SecretModel
{
    public string? Value { get; set; }

    public string? Description { get; set; }

    public DateTime? Expiration { get; set; }

    public string Type { get; set; } = "SharedSecret";

    public SecretModel(string? value = null, string? description = null, DateTime? expiration = null)
    {
        Value = value;
        Description = description;
        Expiration = expiration;
    }

    public override int GetHashCode()
    {
        return (17 * 23 + (Value?.GetHashCode() ?? 0)) * 23 + Type.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj is SecretModel secret)
        {
            if (secret == this) return true;
            return string.Equals(secret.Type, Type, StringComparison.Ordinal) && string.Equals(secret.Value, Value, StringComparison.Ordinal);
        }

        return false;
    }
}

