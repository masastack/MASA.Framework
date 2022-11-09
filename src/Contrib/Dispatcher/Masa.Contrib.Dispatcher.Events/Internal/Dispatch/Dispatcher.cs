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
            if (IsSagaMode(type, method))
                return;

            var attribute = method.GetCustomAttributes(typeof(EventHandlerAttribute), true).FirstOrDefault();
            if (attribute is EventHandlerAttribute handler)
            {
                var parameters = method.GetParameters().Where(parameter => typeof(IEvent).IsAssignableFrom(parameter.ParameterType))
                    .ToList();

                if (parameters.Count != 1)
                    throw new ArgumentOutOfRangeException(
                        $"[{method.Name}] only allows one parameter and inherits from Event, other parameters must support getting from DI");

                var parameter = parameters[0];

                handler.ActionMethodInfo = method;
                handler.InstanceType = type;
                handler.EventType = parameter.ParameterType;
                handler.BuildExpression();
                AddRelationNetwork(parameter.ParameterType, handler);
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex,
                "Dispatcher: Failed to get EventBus network, type name: [{TypeName}], method: [{MethodName}]",
                (type.FullName ?? type.Name),
                method.Name);
            throw;
        }
    }
}
