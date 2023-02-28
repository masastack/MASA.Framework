// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs;

public interface IBackgroundJobStorage
{
    Task InsertAsync(BackgroundJobInfo jobInfo);

    Task<List<BackgroundJobInfo>> RetrieveJobsAsync(int batchSize);

    Task DeleteAsync(Guid id);

    Task UpdateAsync(BackgroundJobInfo jobInfo);
}
