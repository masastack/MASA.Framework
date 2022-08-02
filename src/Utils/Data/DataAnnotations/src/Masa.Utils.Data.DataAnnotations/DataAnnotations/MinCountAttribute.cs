// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System.ComponentModel.DataAnnotations;

public class MinCountAttribute : ValidationAttribute
{
    private const string DEFAULT_ERROR_MESSAGE = "The field {0} must be a list type with a minimum count of '{1}'.";

    public MinCountAttribute(int count) : base(DEFAULT_ERROR_MESSAGE)
    {
        Count = count;
    }

    public int Count { get; }

    public override bool IsValid(object? value)
    {
        // Check the lengths for legality
        EnsureLegalLengths();

        int count;

        if (value == null)
        {
            return true;
        }

        if (value is IList list)
        {
            count = list.Count;
        }
        else
        {
            throw new InvalidCastException($"The field of type {value.GetType()} must be a list type.");
        }

        return count >= Count;
    }

    public override string FormatErrorMessage(string name)
    {
        return string.Format(ErrorMessageString, name, Count);
    }

    /// <summary>
    /// Checks that Length has a legal value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Length is less than zero.</exception>
    private void EnsureLegalLengths()
    {
        if (Count < 0)
        {
            throw new InvalidOperationException($"{nameof(MinCountAttribute)} must have a Count value that is zero or greater.");
        }
    }
}
