namespace MASA.Contrib.Dispatcher.Events.Internal.Dispatch;

internal class SagaDispatcher : DispatcherBase
{
    public SagaDispatcher(IServiceCollection services, bool forceInit = false) : base(services, forceInit) { }

    public SagaDispatcher Build(ServiceLifetime lifetime, params Assembly[] assemblies)
    {
        AddSagaDispatchRelation(_services, typeof(IEventHandler<>), lifetime, assemblies);
        AddSagaDispatchRelation(_services, typeof(ISagaEventHandler<>), lifetime, assemblies);
        return this;
    }

    private IServiceCollection AddSagaDispatchRelation(IServiceCollection services, Type eventBusHandlerType, ServiceLifetime lifetime, params Assembly[] assemblies)
    {
        foreach (var item in GetAddSagaServices(eventBusHandlerType, assemblies))
        {
            services.Add(item.ServiceType, item.ImplementationType, lifetime);
            AddSagaRelationNetwork(item.ImplementationType);
        }
        return services;
    }

    private void AddSagaRelationNetwork(Type eventBusHandlerType)
    {
        var eventHandlers = GetSagaHandlers(eventBusHandlerType);
        var eventHandler = eventHandlers.Where(x => x.Order != int.MaxValue).FirstOrDefault();
        var actualOrder = eventHandler?.Order ?? int.MaxValue;
        if (actualOrder < 0)
        {
            throw new ArgumentOutOfRangeException("The order must be greater than or equal to 0");
        }
        foreach (var handler in eventHandlers)
        {
            if (actualOrder != handler.Order)
            {
                handler.Order = actualOrder;
            }
            AddRelationNetwork(handler.EventType, handler);
        }
    }

    private List<EventHandlerAttribute> GetSagaHandlers(Type eventBusHandlerType)
    {
        var methods = eventBusHandlerType.GetMethods();
        Type? eventType = null!;
        List<EventHandlerAttribute> eventHandlers = new();
        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            if (parameters != null && parameters.Length == 1 && parameters.All(parameter => typeof(IEvent).IsAssignableFrom(parameter.ParameterType) && !typeof(IIntegrationEvent).IsAssignableFrom(parameter.ParameterType)) && IsSagaMode(eventBusHandlerType, method))
            {
                var attribute = method.GetCustomAttributes(typeof(EventHandlerAttribute), true).FirstOrDefault();
                var handler = attribute != null ? attribute as EventHandlerAttribute : null;
                if (eventType == null)
                {
                    eventType = parameters.Select(x => x.ParameterType).FirstOrDefault()!;
                }

                if (handler is null)
                {
                    handler = new EventHandlerAttribute();
                }
                handler.ActionMethodInfo = method;
                handler.InstanceType = eventBusHandlerType;
                handler.EventType = eventType;
                handler.IsCancel = method.Name.Equals(nameof(ISagaEventHandler<IEvent>.CancelAsync));
                eventHandlers.Add(handler);
            }
        }

        //In saga mode, when the user sets an Order for either HandlerAsync or CancelAsync
        //the default int.MaxValue Order will be overridden
        //but when the Order of HandlerAsync and CancelAsync are inconsistent
        //an error will be displayed
        if (eventHandlers.Where(handler => handler.Order != int.MaxValue).Select(handler => handler.Order).Distinct().Count() > 1)
        {
            throw new ArgumentException($"In saga mode {nameof(IEventHandler<IEvent>.HandleAsync)} needs to be the same as {nameof(ISagaEventHandler<IEvent>.CancelAsync)} Order");
        }
        return eventHandlers;
    }

    private List<(Type ServiceType, Type ImplementationType)> GetAddSagaServices(Type eventBusHandlerType, params Assembly[] assemblies)
    {
        List<(Type ServiceType, Type ImplementationType)> list = new();
        var serviceTypeAndImplementationInfo = GetSagaServiceTypeAndImplementations(eventBusHandlerType, assemblies);
        foreach (var serviceType in serviceTypeAndImplementationInfo.ServiceTypeList)
        {
            var implementationTypes = serviceTypeAndImplementationInfo.ImplementationType.Where(implementationType => serviceType.IsAssignableFrom(implementationType)).ToList();

            foreach (var implementationType in implementationTypes)
            {
                list.Add((serviceType, implementationType));
            }
        }

        return list;
    }

    private (List<Type> ServiceTypeList, List<Type> ImplementationType) GetSagaServiceTypeAndImplementations(Type eventBusHandlerType, params Assembly[] assemblies)
    {
        var concretions = new List<Type>();
        var interfaces = new List<Type>();
        foreach (var type in assemblies.SelectMany(a => a.DefinedTypes).Where(t => !t.IsGeneric()))
        {
            if (type.IsConcrete())
            {
                concretions.Add(type);
            }

            if (eventBusHandlerType.IsGenericInterfaceAssignableFrom(type) && !interfaces.Contains(type))
            {
                interfaces.AddRange(type.GetInterfaces());
            }
        }
        return (interfaces, concretions);
    }
}
