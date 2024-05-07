// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using FluentValidation.AspNetCore;
using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDaprStarter();//builder.Configuration.GetSection("DaprStarter")
}

// Add services to the container.

builder.Services.AddControllers().AddDapr();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidation(options =>
{
    options.RegisterValidatorsFromAssemblyContaining<User>();
});
builder.Services.AddDomainEventBus(dispatcherOptions =>
{
    dispatcherOptions
        .UseIntegrationEventBus(option =>
        {
            option.BatchesGroupSendOrRetry = true;
            option.UseDapr().UseEventLog<CustomDbContext>();
        })//
        .UseEventBus(eventBusBuilder => eventBusBuilder.UseMiddleware(typeof(RecordEventMiddleware<>)).UseMiddleware(typeof(ValidatorEventMiddleware<>)))
        .UseUoW<CustomDbContext>(optionBuilder =>
        {
            optionBuilder.UseSqlite($"data source=disabled-soft-delete-db-030e2f29-3398-493e-831a-0df85d3cba90").UseFilter();
            //optionBuilder.UseSqlite($"data source=disabled-soft-delete-db-{Guid.NewGuid()}").UseFilter();
        })
        .UseRepository<CustomDbContext>();
});
var dbContext = builder.Services.BuildServiceProvider().GetRequiredService<CustomDbContext>();
dbContext.Database.EnsureCreated();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCloudEvents();
app.MapControllers();
app.MapSubscribeHandler();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
