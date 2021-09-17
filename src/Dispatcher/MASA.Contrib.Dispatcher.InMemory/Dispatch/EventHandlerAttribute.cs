using MASA.Contrib.Dispatcher.InMemory.Internal.Expressions;

namespace MASA.Contrib.Dispatcher.InMemory.Dispatch;

[AttributeUsage(AttributeTargets.Method)]
public class EventHandlerAttribute : Attribute
{
    public EventHandlerAttribute() : this(DefaultOrder)
    {
    }

    public EventHandlerAttribute(int order) : this(order, false)
    {

    }

    public EventHandlerAttribute(int order, bool enableRetry) : this(order, enableRetry, enableRetry ? DefaultRetryCount : 0)
    {

    }

    public EventHandlerAttribute(int order, FailureLevels failureLevels) : this(order, failureLevels, false)
    {

    }

    public EventHandlerAttribute(int order, bool enableRetry, int retryTimes) : this(order, FailureLevels.Throw, enableRetry, retryTimes)
    {

    }

    public EventHandlerAttribute(int order, bool enableRetry, bool isCancel) : this(order, enableRetry, isCancel, enableRetry ? DefaultRetryCount : 0)
    {

    }

    public EventHandlerAttribute(int order, bool enableRetry, bool isCancel, int retryTimes) : this(order, FailureLevels.Throw, enableRetry, retryTimes, isCancel)
    {

    }

    public EventHandlerAttribute(int order, FailureLevels failureLevels, bool enableRetry) : this(order, failureLevels, enableRetry, enableRetry ? DefaultRetryCount : 0)
    {

    }

    public EventHandlerAttribute(int order, FailureLevels failureLevels, bool enableRetry, bool isCancel) : this(order, failureLevels, enableRetry, enableRetry ? DefaultRetryCount : 0, isCancel)
    {

    }

    public EventHandlerAttribute(int order, FailureLevels failureLevels, bool enableRetry, int retryTimes) : this(order, failureLevels, enableRetry, retryTimes, false)
    {

    }

    public EventHandlerAttribute(int order, FailureLevels failureLevels, bool enableRetry, int retryTimes, bool isCancel)
    {
        this.Order = order;
        this.FailureLevels = failureLevels;
        this.EnableRetry = enableRetry;
        this.RetryTimes = enableRetry ? retryTimes : 0;
        this.IsCancel = isCancel;
    }

    /// <summary>
    /// The default number of retry attempts.
    /// </summary>
    private const int DefaultRetryCount = 3;

    private const int DefaultOrder = int.MaxValue;

    /// <summary>
    /// Used to control the order in which methods are executed, in ascending order. default is 100
    /// Must be greater than or equal to 0
    /// </summary>
    public int Order { get; set; }

    public FailureLevels FailureLevels { get; set; }

    public bool EnableRetry { get; set; }

    /// <summary>
    /// The default value is 3，EnableRetry must be true to take effect
    /// </summary>
    public int RetryTimes { get; set; }

    /// <summary>
    /// Used to cancel or compensate
    /// </summary>
    public bool IsCancel { get; set; }

    internal MethodInfo ActionMethodInfo { get; set; }

    internal Type InstanceType { get; set; }

    internal Type EventType { get; set; }

    internal int ActualRetryTimes => EnableRetry ? RetryTimes : 0;

    internal TaskInvokeDelegate InvokeDelegate { get; private set; }

    private object Instance { get; set; } = default!;

    private object EventHandler { get; set; }

    internal bool IsEventHandler => FailureLevels == FailureLevels.Throw || FailureLevels == FailureLevels.ThrowAndCancel;

    internal void BuildExpression()
    {
        InvokeDelegate = InvokeBuilder.Build(ActionMethodInfo, InstanceType);
    }

    internal async Task ExcuteAction<TEvent>(IServiceProvider serviceProvider, TEvent @event) where TEvent : IEvent
    {
        if (InvokeDelegate != null)
        {
            Instance = serviceProvider.GetRequiredService(InstanceType);
            await InvokeDelegate.Invoke(Instance, @event);
        }
        else
        {
            await ExcuteSagaAction(serviceProvider, @event);
        }
    }

    private async Task ExcuteSagaAction<TEvent>(IServiceProvider serviceProvider, TEvent @event) where TEvent : IEvent
    {
        if (!IsCancel)
        {
            if (EventHandler == null)
            {
                var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>();
                var handler = handlers.Where(x => x.GetType().Equals(InstanceType)).FirstOrDefault()!;
                EventHandler = handler;
            }
            await ((IEventHandler<TEvent>)EventHandler).HandleAsync(@event);
        }
        else
        {
            if (EventHandler == null)
            {
                var handlers = serviceProvider.GetServices<ISagaEventHandler<TEvent>>();
                var handler = handlers.Where(x => x.GetType().Equals(InstanceType)).FirstOrDefault()!;
                EventHandler = handler;
            }
            await ((ISagaEventHandler<TEvent>)EventHandler).CancelAsync(@event);
        }
    }

    internal bool IsHandlerMissing(int maxCancelOrder) => (FailureLevels == FailureLevels.ThrowAndCancel && Order < maxCancelOrder) || (FailureLevels == FailureLevels.Throw && Order <= maxCancelOrder);
}
