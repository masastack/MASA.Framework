// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal class StringBidirectionalDictionary : BidirectionalDictionary<string, string>
{

    internal StringBidirectionalDictionary() : base() { }

    internal StringBidirectionalDictionary(Dictionary<string, string> firstToSecondDictionary)
        : base(firstToSecondDictionary)
    { }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    internal override bool ExistsInFirst(string value)
    {
        return base.ExistsInFirst(value.ToLowerInvariant());
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    internal override bool ExistsInSecond(string value)
    {
        return base.ExistsInSecond(value.ToLowerInvariant());
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    internal override string? GetFirstValue(string value)
    {
        return base.GetFirstValue(value.ToLowerInvariant());
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    internal override string? GetSecondValue(string value)
    {
        return base.GetSecondValue(value.ToLowerInvariant());
    }

}
