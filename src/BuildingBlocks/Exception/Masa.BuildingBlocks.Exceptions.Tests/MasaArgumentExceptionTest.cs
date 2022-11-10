// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Exceptions.Tests;

[TestClass]
public class MasaArgumentExceptionTest
{
    [TestMethod]
    public void TestThrowIfNull()
    {
        object? str = null;

        try
        {
            MasaValidatorException.ThrowIfNull("");

            MasaArgumentException.ThrowIfNull(str);
        }
        catch (MasaArgumentException ex)
        {
            Assert.AreEqual(Data.Constants.ErrorCode.NOT_NULL_VALIDATOR, ex.ErrorCode);
            Assert.AreEqual("'{0}' must not be empty.", ex.GetErrorMessage());
        }
    }
}
