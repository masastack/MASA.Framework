// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs.Memory;

public class RegisterAccountBackgroundJob : IBackgroundJob<RegisterAccountParameter>
{
    public Task ExecuteAsync(RegisterAccountParameter args)
    {
        return Task.CompletedTask;
    }
}
