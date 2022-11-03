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
            MasaArgumentException.ThrowIfNull(str);
        }
        catch (MasaArgumentException ex)
        {
            Assert.AreEqual(Masa.BuildingBlocks.Data.Constants.ErrorCode.ARGUMENT_NULL, ex.ErrorCode);
            Assert.AreEqual("Value cannot be null. (Parameter '{0}')", ex.ErrorMessage);
        }
    }
}
