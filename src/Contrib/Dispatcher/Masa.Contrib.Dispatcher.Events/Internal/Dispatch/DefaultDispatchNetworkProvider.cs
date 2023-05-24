// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal.Dispatch;

internal class DefaultDispatchNetworkProvider : IDispatchNetworkProvider
{
    private readonly Assembly[] _assemblies;
    private readonly List<EventHandlerAttribute> _handlerNetwork;
    private readonly List<EventHandlerAttribute> _cancelHandlerNetwork;

    public IReadOnlyList<EventHandlerAttribute> HandlerNetwork => _handlerNetwork;

    public IReadOnlyList<EventHandlerAttribute> CancelHandlerNetwork => _cancelHandlerNetwork;

    private readonly ILogger<DefaultDispatchNetworkProvider>? _logger;

    public DefaultDispatchNetworkProvider(Assembly[] assemblies, ILoggerFactory? loggerFactory)
    {
        _assemblies = assemblies;
        _logger = loggerFactory?.CreateLogger<DefaultDispatchNetworkProvider>();
        _handlerNetwork = new List<EventHandlerAttribute>();
        _cancelHandlerNetwork = new List<EventHandlerAttribute>();
        BuildDispatchNetwork();
    }

    private void BuildDispatchNetwork()
    {
        foreach (var assembly in _assemblies)
        {
            BuildDispatchNetwork(assembly);
        }
    }

    private void BuildDispatchNetwork(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsConcrete())
                continue; //Handler and Cancel must be normal classes, not abstract classes or interfaces

            foreach (var method in type.GetMethods())
            {
                var eventHandlerAttribute = TryAddRelationNetwork(type, method);

                if (eventHandlerAttribute == null)
                    continue;

                if (eventHandlerAttribute.IsCancel)
                {
                    _cancelHandlerNetwork.Add(eventHandlerAttribute);
                }
                else
                {
                    _handlerNetwork.Add(eventHandlerAttribute);
                }
            }
        }
    }

    private EventHandlerAttribute? TryAddRelationNetwork(Type type, MethodInfo method)
    {
        try
        {
            if (DispatchNetworkUtils.IsSagaMode(type, method))
                return null;

            var attribute = method.GetCustomAttributes(typeof(EventHandlerAttribute), true).FirstOrDefault();
            if (attribute is not EventHandlerAttribute handler)
                return null;

            var parameters = method
                .GetParameters()
                .Where(parameter => typeof(IEvent).IsAssignableFrom(parameter.ParameterType))
                .ToList();

            if (parameters.Count != 1)
                throw new ArgumentOutOfRangeException(
                    $"[{method.Name}] only allows one parameter and inherits from Event, other parameters must support getting from DI");

            var parameter = parameters[0];

            handler.ActionMethodInfo = method;
            handler.InstanceType = type;
            handler.EventType = parameter.ParameterType;
            handler.BuildExpression();

            return handler;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex,
                "Dispatcher: Failed to get EventBus network, type name: [{TypeName}], method: [{MethodName}]",
                type.FullName ?? type.Name,
                method.Name);
            throw;
        }
    }
}
