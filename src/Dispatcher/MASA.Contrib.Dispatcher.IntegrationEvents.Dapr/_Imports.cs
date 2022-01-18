global using Dapr.Client;
global using MASA.BuildingBlocks.Data.UoW;
global using MASA.BuildingBlocks.Dispatcher.Events;
global using MASA.BuildingBlocks.Dispatcher.IntegrationEvents;
global using MASA.BuildingBlocks.Dispatcher.IntegrationEvents.Logs;
global using MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Options;
global using MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;
global using MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Servers;
global using MASA.Utils.Models.Config;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using System.Reflection;
global using System.Text.Json.Serialization;

