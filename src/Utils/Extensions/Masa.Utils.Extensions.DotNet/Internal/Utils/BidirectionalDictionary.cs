//---------------------------------------------------------------------
// <copyright file="BidirectionalDictionary.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

/// <summary>
/// This class provide service for both the singularization and pluralization, it takes the word pairs
/// in the ctor following the rules that the first one is singular and the second one is plural.
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal class BidirectionalDictionary<TFirst, TSecond>
    where TFirst : notnull
    where TSecond : notnull
{
    internal Dictionary<TFirst, TSecond> FirstToSecondDictionary { get; set; }
    internal Dictionary<TSecond, TFirst> SecondToFirstDictionary { get; set; }

    internal BidirectionalDictionary()
    {
        FirstToSecondDictionary = new Dictionary<TFirst, TSecond>();
        SecondToFirstDictionary = new Dictionary<TSecond, TFirst>();
    }

    internal BidirectionalDictionary(Dictionary<TFirst, TSecond> firstToSecondDictionary) : this()
    {
        foreach (var key in firstToSecondDictionary.Keys)
        {
            AddValue(key, firstToSecondDictionary[key]);
        }
    }

    internal virtual bool ExistsInFirst(TFirst value)
    {
        return FirstToSecondDictionary.ContainsKey(value);
    }

    internal virtual bool ExistsInSecond(TSecond value)
    {
        return SecondToFirstDictionary.ContainsKey(value);
    }

    internal virtual TSecond? GetSecondValue(TFirst value)
    {
        if (ExistsInFirst(value))
            return FirstToSecondDictionary[value];

        return default;
    }

    internal virtual TFirst? GetFirstValue(TSecond value)
    {
        if (ExistsInSecond(value))
            return SecondToFirstDictionary[value];

        return default;
    }

    internal virtual void AddValue(TFirst firstValue, TSecond secondValue)
    {
        FirstToSecondDictionary.Add(firstValue, secondValue);

        if (!SecondToFirstDictionary.ContainsKey(secondValue))
        {
            SecondToFirstDictionary.Add(secondValue, firstValue);
        }
    }
}
