// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace Masa.BuildingBlocks.Exceptions.Tests;

[TestClass]
public class UserFriendlyExceptionTest
{
    [TestMethod]
    public void Test()
    {
        var exception = new UserFriendlyException("user friendly exception");
        Assert.ThrowsExactly<UserFriendlyException>(() => throw exception, "user friendly exception");
    }
}
