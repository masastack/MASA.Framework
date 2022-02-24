namespace Masa.Contrib.Dispatcher.Events.Internal.Dispatch;

internal class DispatchRelationNetwork
{
    public Dictionary<Type, List<DispatchRelationOptions>> RelationNetwork { get; set; } = new();

    public Dictionary<Type, List<EventHandlerAttribute>> HandlerRelationNetwork { get; set; } = new();

    public Dictionary<Type, List<EventHandlerAttribute>> CancelRelationNetwork { get; set; } = new();

    private readonly ILogger<DispatchRelationNetwork>? _logger;

    public DispatchRelationNetwork(ILogger<DispatchRelationNetwork>? logger) => _logger = logger;

    public void Add(Type keyEventType, EventHandlerAttribute handler)
    {
        Add(keyEventType, handler, !handler.IsCancel ? HandlerRelationNetwork : CancelRelationNetwork);
    }

    /// <summary>
    /// If the relationship does not exist in the network, add it
    /// </summary>
    /// <param name="keyEventType"></param>
    /// <param name="handlers"></param>
    /// <param name="dispatchRelativeNetwork"></param>
    private void Add(Type keyEventType,
        EventHandlerAttribute handlers,
        Dictionary<Type, List<EventHandlerAttribute>> dispatchRelativeNetwork)
    {
        if (!dispatchRelativeNetwork.ContainsKey(keyEventType))
        {
            dispatchRelativeNetwork.Add(keyEventType, new List<EventHandlerAttribute>());
        }

        if (!dispatchRelativeNetwork[keyEventType].Any(x => x.ActionMethodInfo.Equals(handlers.ActionMethodInfo) && x.InstanceType.Equals(handlers.InstanceType)))
        {
            dispatchRelativeNetwork[keyEventType].Add(handlers);
        }
    }

    public void Build()
    {
        Sort();
        CheckConstraints();
        RelationNetwork = HandlerRelationNetwork.ToDictionary(relationNetwork => relationNetwork.Key,
            relationNetwork => relationNetwork.Value.Select(handler => new DispatchRelationOptions(handler)).ToList());

        foreach (var relation in RelationNetwork)
        {
            foreach (var relationOption in RelationNetwork[relation.Key]!)
            {
                if (CancelRelationNetwork.TryGetValue(relation.Key, out List<EventHandlerAttribute>? cancelRelations))
                {
                    var cancelHandlers = cancelRelations.TakeWhile(handler => relationOption.IsCancelHandler(relationOption.Handler)).Reverse().ToList();
                    relationOption.AddCancelHandler(cancelHandlers);
                }
            }
        }
    }

    private void Sort()
    {
        HandlerRelationNetwork = Sort(HandlerRelationNetwork);
        CancelRelationNetwork = Sort(CancelRelationNetwork);
    }

    private Dictionary<Type, List<TDispatchHandlerAttribute>> Sort<TDispatchHandlerAttribute>(Dictionary<Type, List<TDispatchHandlerAttribute>> dispatchRelatives)
        where TDispatchHandlerAttribute : EventHandlerAttribute
    {
        return dispatchRelatives.ToDictionary(
            dispatchRelative => dispatchRelative.Key,
            dispatchRelative => dispatchRelative.Value.OrderBy(attr => attr.Order).ToList()
        );
    }

    /// <summary>
    /// Checking scheduling Relationships
    /// Throw an exception for a Handler that only has Cancel
    /// and warn a Handler that the Cancel will never perform because the Order is improperly set
    /// </summary>
    private void CheckConstraints()
    {
        foreach (var cancelRelation in CancelRelationNetwork)
        {
            if (HandlerRelationNetwork.All(relation => relation.Key != cancelRelation.Key))
            {
                throw new NotSupportedException($"{cancelRelation.Key.Name} is only have a cancel handler, it must have an event handler.");
            }

            var maxCancelOrder = cancelRelation.Value.Max(handler => handler.Order);
            var maxHandlerOrder = HandlerRelationNetwork[cancelRelation.Key].Where(handler => handler.IsEventHandler).OrderByDescending(handler => handler.Order).ThenByDescending(handler => handler.FailureLevels).FirstOrDefault();
            if (maxHandlerOrder == null || maxHandlerOrder.IsHandlerMissing(maxCancelOrder))
            {
                var methodName = cancelRelation.Value.Select(x => x.ActionMethodInfo.Name).LastOrDefault();
                _logger?.LogWarning($"The {methodName} method is meaningless, because its Order attribute is too large, and no handler corresponding to the Order can be triggered. It is suggested to lower the Order attribute of {methodName} or add a matching handler - {cancelRelation.Value.Select(x => x.InstanceType.FullName).LastOrDefault()}");
            }
        }
    }
}
