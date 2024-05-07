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
    public class IntegrationEventLogController : ControllerBase
    {
        private readonly ILogger<IntegrationEventLogController> _logger;
        private readonly IEventBus eventBus;
        private readonly CustomDbContext dbContext;
        private readonly IUnitOfWork unitOfWork;
        private readonly Masa.Framework.EventApiTest.Domain.IUserRepository userRepository;
        private readonly IIntegrationEventBus integrationEventBus;
        private readonly DaprClient DaprClient;

        public IntegrationEventLogController(ILogger<IntegrationEventLogController> logger, IEventBus eventBus, CustomDbContext customDbContext,
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

        [HttpPost("/createOrder")]
        public async Task<ActionResult> CreateOrderAsync()
        {
            var order = new
            {
                orderId = "123456",
                productId = "67890",
                amount = 2
            };

            await DaprClient.PublishEventAsync<object>("pubsub", "newOrder", order);
            return new JsonResult(order);
        }

        [HttpPost("/bulk_createOrder")]
        public async Task<ActionResult> BulkCreateOrder2()
        {
            using var client = new DaprClientBuilder().Build();
            var events = new List<object>()
            {
                new
                {
                    orderId = "123456",
                    productId = "67890",
                    amount = 1
                },new{
                    orderId = "123456",
                    productId = "67890",
                    amount = 2
                }
            };

            var res = await client.BulkPublishEventAsync("pubsub", "newOrder", events);
            if (res == null)
            {
                throw new Exception("null response from dapr");
            }

            if (res.FailedEntries.Count > 0)
            {
                Console.WriteLine("Some events failed to be published!");
                foreach (var failedEntry in res.FailedEntries)
                {
                    Console.WriteLine("EntryId: " + failedEntry.Entry.EntryId + " Error message: " +
                                      failedEntry.ErrorMessage);
                }
            }
            else
            {
                Console.WriteLine("Published all events!");
            }

            return new JsonResult(events);
        }

        //[Dapr.AspNetCore.BulkSubscribe("newOrder", 10, 10)]
        [Topic("pubsub", "newOrder")]
        [HttpPost("/createOrderHandler")]
        public async Task<ActionResult> CreateOrderHandler(object data)
        {
            return Content(JsonConvert.SerializeObject(data));
        }

        [HttpGet]
        public async Task<long> AddUser()
        {
            var user = new User
            {
                Age = 18,
                Name = 1.ToString()
            };
            user.RegisterUserDomainEvent();

            await userRepository.AddAsync(user);
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitAsync();
            var count = await userRepository.GetCountAsync();
            await Console.Out.WriteLineAsync(count.ToString());
            return count;
        }

        [HttpGet]
        public async Task<long> AddAsync()
        {
            var logs = new List<IntegrationEventLog>();

            logs.Add(new IntegrationEventLog(new TestBaseEventLocal(1), Guid.Parse("D98F4EA8-8EDE-48BA-A767-2863614AE34C")));
            logs.Add(new IntegrationEventLog(new TestBaseEventLocal(2), Guid.Parse("D98F4EA8-8EDE-48BA-A767-2863614AE34C")));
            await dbContext.Set<IntegrationEventLog>().AddRangeAsync(logs);
            dbContext.SaveChanges();
            var count = await dbContext.Set<IntegrationEventLog>().CountAsync();

            return count;
        }

        [HttpGet]
        public async Task<long> UpdateStateAsync(IntegrationEventStates state = IntegrationEventStates.NotPublished)
        {
            var logs = await dbContext.Set<IntegrationEventLog>().ToListAsync();

            foreach (var item in logs)
            {
                item.State = state;
            }

            dbContext.Set<IntegrationEventLog>().UpdateRange(logs);
            dbContext.SaveChanges();
            return logs.Count;
        }

        [HttpGet]
        public async Task<List<IntegrationEventLog>> GetListAsync()
        {
            return await dbContext.Set<IntegrationEventLog>().ToListAsync();
        }

        [HttpGet]
        public async Task<long> PublishAsync(long idnex = 1)
        {
            await integrationEventBus.PublishAsync(new TestBaseEventLocal(idnex));
            return idnex;
        }

        [HttpPost]
        [Topic("pubsub", nameof(AddGoodsIntegrationEvent))]
        public async Task AddGoodsIntegrationEventHandlerAsync(AddGoodsIntegrationEvent integrationEvent)
        {
            await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(integrationEvent));
        }

        [HttpPost]
        [Topic("pubsub", nameof(TestBaseEventLocal))]
        public async Task TestBaseEventLocalHandlerAsync(TestBaseEventLocal integrationEvent)
        {
            await Console.Out.WriteLineAsync(integrationEvent.Index.ToString());
        }
    }
}
