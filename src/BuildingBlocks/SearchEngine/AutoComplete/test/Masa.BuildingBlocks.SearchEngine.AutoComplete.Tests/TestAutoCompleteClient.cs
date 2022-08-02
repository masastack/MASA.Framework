// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete.Tests;

[TestClass]
public class TestAutoCompleteClient
{
    [TestMethod]
    public async Task TestDeleteMultiAsyncReturnSuccess()
    {
        var client = new CustomAutoCompleteClient();
        var response = await client.DeleteAsync(new[] { 1, 2 });
        Assert.IsTrue(response.IsValid);

        response = await client.DeleteAsync(new[] { "1", "2" });
        Assert.IsTrue(response.IsValid);

        response = await client.DeleteAsync(new[] { 1d, 2d });
        Assert.IsTrue(response.IsValid);

        response = await client.DeleteAsync(new[] { 1L, 2L });
        Assert.IsTrue(response.IsValid);

        response = await client.DeleteAsync(new[] { Guid.NewGuid(), Guid.NewGuid() });
        Assert.IsTrue(response.IsValid);
    }
}
