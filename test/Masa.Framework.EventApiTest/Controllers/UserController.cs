// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Dapr;
using Dapr.AspNetCore;
using Dapr.Client;
using Masa.BuildingBlocks.Dispatcher.IntegrationEvents.Logs;
using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Masa.Framework.EventApiTest.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<IntegrationEventLogController> _logger;
        private readonly IEventBus eventBus;
        private readonly CustomDbContext dbContext;
        private readonly IUnitOfWork unitOfWork;
        private readonly Masa.Framework.EventApiTest.Domain.IUserRepository userRepository;
        private readonly IIntegrationEventBus integrationEventBus;
        private readonly DaprClient DaprClient;

        public UserController(ILogger<IntegrationEventLogController> logger, IEventBus eventBus, CustomDbContext customDbContext,
            IUnitOfWork unitOfWork, Domain.IUserRepository userRepository, IIntegrationEventBus integrationEventBus, DaprClient daprClient)
        {
            _logger = logger;
            this.eventBus = eventBus;
            this.dbContext = customDbContext;
            this.unitOfWork = unitOfWork;
            this.userRepository = userRepository;
            this.integrationEventBus = integrationEventBus;
            this.DaprClient = daprClient;
        }

        [HttpPost]
        public async Task<User> EntityCreatedDomainEventAsync()
        {
            var user = new User
            {
                Age = 18,
                Name = 1.ToString(),
            };

            await userRepository.AddAsync(user);
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitAsync();
            return user;
        }

        [HttpPut]
        public async Task<User> EntityModifiedDomainEventAsync()
        {
            var user = await userRepository.FindAsync(item => item.Id != Guid.Empty);
            user.Age++;
            await userRepository.UpdateAsync(user);
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitAsync();
            return user;
        }

        [HttpDelete]
        public async Task<long> EntityDeletedDomainEventAsync()
        {
            var user = await userRepository.FindAsync(item => item.Id != Guid.Empty);

            await userRepository.RemoveAsync(user);
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitAsync();
            return await userRepository.GetCountAsync();
        }
    }
}
