// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.EventBus.IntegrationTests.Application.Queries;

public class UserAgeQueryValidator: AbstractValidator<UserAgeQuery>
{
    public UserAgeQueryValidator()
    {
        RuleFor(u => u.Name).NotNull().WithMessage("Name is required on UserAgeQuery");
    }
}
