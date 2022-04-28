// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal.Dispatch;

internal class Dispatcher : DispatcherBase
{
    public Dispatcher(IServiceCollection services, Assembly[] assemblies, bool forceInit = false) : base(services, assemblies, forceInit)
    {
    }

    public Dispatcher Build(ServiceLifetime lifetime)
    {
        foreach (var assembly in Assemblies)
        {
            AddRelationNetwork(assembly);
        }
        foreach (var dispatchInstance in GetAddServiceTypeList())
        {
            Services.Add(dispatchInstance, dispatchInstance, lifetime);
        }
        Build();
        return this;
    }

    private void AddRelationNetwork(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsConcrete())
            {
                continue; //Handler and Cancel must be normal classes, not abstract classes or interfaces
            }

            foreach (var method in type.GetMethods())
            {
                AddRelationNetwork(type, method);
            }
        }
    }

    private void AddRelationNetwork(Type type, MethodInfo method)
    {
        try
        {
            var attribute = method.GetCustomAttributes(typeof(EventHandlerAttribute), true).FirstOrDefault();
            if (attribute is not null && attribute is EventHandlerAttribute handler)
            {
                var parameters = method.GetParameters();

                if (parameters == null || parameters.Length != 1 ||
                    !parameters.Any(parameter => typeof(IEvent).IsAssignableFrom(parameter.ParameterType)))
                    throw new ArgumentOutOfRangeException($"[{method.Name}] must have only one argument and inherit from Event");

                if (IsSagaMode(type, method))
                    return;

                var parameter = parameters.FirstOrDefault()!;
                handler.ActionMethodInfo = method;
                handler.InstanceType = type;
                handler.EventType = parameter.ParameterType;
                handler.BuildExpression();
                AddRelationNetwork(parameter.ParameterType, handler);
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError($"Dispatcher: Failed to get EventBus network, type name: [{type.FullName ?? type.Name}], method: [{method.Name}]", ex);
            throw;
        }
    }
}
