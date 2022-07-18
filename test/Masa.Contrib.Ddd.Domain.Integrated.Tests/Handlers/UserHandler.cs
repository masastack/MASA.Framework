// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Integrated.Tests.Handlers;

public class UserHandler
{
    private readonly IRepository<User, Guid> _repository;

    public UserHandler(IRepository<User, Guid> repository)
        => _repository = repository;

    [EventHandler]
    public async Task RegisterUser(RegisterUserEvent @event)
    {
        var user = new User(@event.Id, @event.Name,@event.IsSendDomainEvent);
        await _repository.AddAsync(user);

        if (user.Name.Contains("error"))
        {
            throw new Exception("custom exception");// Throwing exceptions manually
        }
    }

    /// <summary>
    /// todo: Event sequence is sent only for testing aggregate root
    /// </summary>
    [EventHandler]
    public async Task RegisterUserByDomainEvent(RegisterUserDomainEvent @event)
    {
        var user = await _repository.FindAsync(@event.Id);
        if (user != null)
        {
            user.UpdateName("Tom");
            await _repository.UpdateAsync(user);
        }
    }

    [EventHandler]
    public async Task UpdateUser(UpdateUserDomainEvent @event)
    {
        var user = await _repository.FindAsync(@event.Id);
        if (user != null && user.Name == "Tom")
        {
            user.Name = "Tom2";
            await _repository.UpdateAsync(user);
        }
    }
}
