namespace MASA.Contrib.Dispatcher.InMemory.Dispatch;

internal class Dispatcher : DispatcherBase
{
    public Dispatcher(IServiceCollection services, bool forceInit = false) : base(services, forceInit) { }

    public Dispatcher Build(ServiceLifetime lifetime, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            AddRelationNetwork(assembly);
        }
        foreach (var dispatchInstance in base.GetAddServiceTypeList())
        {
            _services.Add(dispatchInstance, dispatchInstance, lifetime);
        }
        base.Build();
        return this;
    }

    private void AddRelationNetwork(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsConcrete())
            {
                continue;//Handler and Cancel must be normal classes, not abstract classes or interfaces
            }

            foreach (var method in type.GetMethods())
            {
                AddRelationNetwork(type, method);
            }
        }
    }

    private void AddRelationNetwork(Type type, MethodInfo method)
    {
        var attribute = method.GetCustomAttributes(typeof(EventHandlerAttribute), true).FirstOrDefault();
        var handler = attribute as EventHandlerAttribute;
        if (attribute is not null && handler is not null)
        {
            var parameters = method.GetParameters();
            if (parameters == null ||
                parameters.Length != 1 ||
                (!parameters.Any(parameter => typeof(IEvent).IsAssignableFrom(parameter.ParameterType))))
            {
                throw new ArgumentOutOfRangeException(string.Format("[{0}] must have only one argument and inherit from Event", method.Name));
            }

            if (IsSagaMode(type, method))
            {
                return;
            }

            if (handler.Order < 0)
            {
                throw new ArgumentOutOfRangeException("The order must be greater than or equal to 0");
            }

            var parameter = parameters.FirstOrDefault()!;
            handler.ActionMethodInfo = method;
            handler.InstanceType = type;
            handler.EventType = parameter.ParameterType;
            handler.BuildExpression();
            AddRelationNetwork(parameter.ParameterType, handler);
        }
    }
}