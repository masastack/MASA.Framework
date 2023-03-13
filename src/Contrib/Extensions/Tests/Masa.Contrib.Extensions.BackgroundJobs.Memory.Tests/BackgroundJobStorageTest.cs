// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs.Memory;

[TestClass]
public class BackgroundJobStorageTest
{
    private BackgroundJobStorage _backgroundJobStorage;

    [TestInitialize]
    public void Initialize()
    {
        _backgroundJobStorage = new();
    }

    [TestMethod]
    public async Task TestStorageAsync()
    {
        var backgroundJobInfo = new BackgroundJobInfo()
        {
            Id = Guid.NewGuid(),
            Name = "masa job"
        };
        Assert.AreEqual("masa job", backgroundJobInfo.Name);

        await _backgroundJobStorage.InsertAsync(backgroundJobInfo);

        var batchSize = 100;
        var list = await _backgroundJobStorage.RetrieveJobsAsync(batchSize);
        Assert.AreEqual(1, list.Count);

        backgroundJobInfo.Name = "new masa job";
        await _backgroundJobStorage.UpdateAsync(backgroundJobInfo);

        list = await _backgroundJobStorage.RetrieveJobsAsync(batchSize);
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("new masa job", list[0].Name);

        await _backgroundJobStorage.DeleteAsync(backgroundJobInfo.Id);
        list = await _backgroundJobStorage.RetrieveJobsAsync(batchSize);
        Assert.AreEqual(0, list.Count);
    }
}
