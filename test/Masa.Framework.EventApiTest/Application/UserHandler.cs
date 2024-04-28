// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Ddd.Domain.Events;

namespace Masa.Framework.IntegrationTests.EventBus.Application;

public class UserHandler
{
    private readonly IEventBus _eventBus;
    private readonly IRepository<User> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UserHandler(IEventBus eventBus, IRepository<User> repository, IUnitOfWork unitOfWork)
    {
        _eventBus = eventBus;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    [EventHandler]
    public async Task RegisterUserByEventAsync(RegisterUserEvent @event)
    {
        await _repository.AddAsync(new User()
        {
            Name = @event.Name,
            Age = @event.Age
        });
    }

    [EventHandler]
    public async Task RegisterUserByCommandAsync(RegisterUserCommand command)
    {
        var query = new CheckUserQuery()
        {
            Name = command.Name
        };
        await _eventBus.PublishAsync(query);
        if (query.Result)
            throw new Exception($"User 【{command.Name}】 already exists");

        await _repository.AddAsync(new User()
        {
            Name = command.Name,
            Age = command.Age
        });
    }

    [EventHandler]
    public async Task UserExistAsync(UserAgeQuery query)
    {
        var checkUserQuery = new CheckUserQuery(); //Check whether the second verification can enter normally
        await Assert.ThrowsExceptionAsync<ValidationException>(async () => await _eventBus.PublishAsync(checkUserQuery), "Name is required on CheckUserQuery");
        if (!checkUserQuery.Result)
            return;

        var user = await _repository.FindAsync(u => u.Name == query.Name);
        query.Result = user!.Age;
    }

    [EventHandler]
    public async Task UserExistAsync(CheckUserQuery query)
    {
        var user = await _repository.FindAsync(u => u.Name == query.Name);
        query.Result = user != null;
    }

    [EventHandler]
    public async Task UserEntityCreatedEventAsync(EntityCreatedDomainEvent<User> command)
    {
        var userEntity = command.Entity;
        if (userEntity is null)
        {
            throw new Exception($"User 【{nameof(UserEntityCreatedEventAsync)}】 already exists");
        }
    }

    [EventHandler]
    public async Task RegisterUserDomainEvent(RegisterUserDomainEvent command)
    {
        await Console.Out.WriteLineAsync($"User 【{nameof(RegisterUserDomainEvent)}】 already exists");
    }

    [EventHandler]
    public async Task RegisterUserIntegrationDomainEvent(RegisterUserIntegrationDomainEvent command)
    {
        await Console.Out.WriteLineAsync($"User 【{nameof(RegisterUserIntegrationDomainEvent)}】 already exists");
    }
}
