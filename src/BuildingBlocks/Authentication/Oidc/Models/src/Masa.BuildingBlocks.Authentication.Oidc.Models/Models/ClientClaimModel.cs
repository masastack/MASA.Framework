// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Oidc.Models.Models;

public class ClientClaimModel
{
    /// <summary>
    /// The claim type
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The claim value
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// The claim value type
    /// </summary>
    public string ValueType { get; set; } = ClaimValueTypes.String;

    /// <summary>
    /// ctor
    /// </summary>
    public ClientClaimModel(string type, string value, string? valueType = null)
    {
        Type = type;
        Value = value;
        if (valueType is not null) ValueType = valueType;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;

            hash = hash * 23 + Value.GetHashCode();
            hash = hash * 23 + Type.GetHashCode();
            hash = hash * 23 + ValueType.GetHashCode();
            return hash;
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj is ClientClaimModel c)
        {
            return string.Equals(Type, c.Type, StringComparison.Ordinal) &&
                    string.Equals(Value, c.Value, StringComparison.Ordinal) &&
                    string.Equals(ValueType, c.ValueType, StringComparison.Ordinal);
        }

        return false;
    }
}

