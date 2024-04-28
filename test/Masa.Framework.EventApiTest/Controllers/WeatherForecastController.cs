// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Dapr;
using Microsoft.AspNetCore.Mvc;

namespace Masa.Framework.EventApiTest.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IEventBus eventBus;
        private readonly CustomDbContext dbContext;
        private readonly IUnitOfWork unitOfWork;
        private readonly Masa.Framework.EventApiTest.Domain.IUserRepository userRepository;
        private readonly IIntegrationEventBus integrationEventBus;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IEventBus eventBus, CustomDbContext customDbContext, IUnitOfWork unitOfWork, Domain.IUserRepository userRepository, IIntegrationEventBus integrationEventBus)
        {
            _logger = logger;
            this.eventBus = eventBus;
            this.dbContext = customDbContext;
            this.unitOfWork = unitOfWork;
            this.userRepository = userRepository;
            this.integrationEventBus = integrationEventBus;
        }

        [HttpGet]
        public async Task<long> Add()
        {
            var user = new User
            {
                Age = 18,
                Name = 1.ToString()
            };
            //user.RegisterUserIntegrationDomainEvent();
            user.RegisterUserDomainEvent();

            await userRepository.AddAsync(user);
            await userRepository.UnitOfWork.SaveChangesAsync();
            await userRepository.UnitOfWork.CommitAsync();

            //await dbContext.Set<User>().AddAsync(user);
            //await unitOfWork.SaveChangesAsync();
            //await unitOfWork.CommitAsync();
            var count = await userRepository.GetCountAsync();
            await Console.Out.WriteLineAsync(count.ToString());

            return count;
        }

        [HttpGet]
        public async Task<long> IntegrationEventBus()
        {
            await integrationEventBus.PublishAsync(new AddGoodsIntegrationEvent()
            {
                Name = "Apple",
                Count = 1,
                Id = Guid.NewGuid(),
                Price = 9.9m,
            });

            return 1;
        }

        [HttpPost]
        [Topic("pubsub", nameof(AddGoodsIntegrationEvent))]
        public async Task UnlockDeviceBindingStatusAsync(AddGoodsIntegrationEvent integrationEvent)
        {
            await Console.Out.WriteLineAsync("123");
        }

    }
}
