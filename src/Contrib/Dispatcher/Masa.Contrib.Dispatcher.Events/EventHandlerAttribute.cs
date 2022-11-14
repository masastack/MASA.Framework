// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events;

[AttributeUsage(AttributeTargets.Method)]
public class EventHandlerAttribute : Attribute
{
    /// <summary>
    /// The default number of retry attempts.
    /// </summary>
    private const int DefaultRetryCount = 3;

    private const int DefaultOrder = int.MaxValue;

    private int _order;

    /// <summary>
    /// Used to control the order in which methods are executed, in ascending order. default is int.MaxValue
    /// Must be greater than or equal to 0
    /// </summary>
    public int Order
    {
        get => _order;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("The order must be greater than or equal to 0");

            _order = value;
        }
    }

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

    private MethodInfo _actionMethodInfo;

    internal MethodInfo ActionMethodInfo
    {
        get => _actionMethodInfo;
        set
        {
            var parameters = value.GetParameters();
            Type[] types = new Type[parameters.Length];

            for (int index = 0; index < types.Length; index++)
            {
                types[index] = parameters[index].ParameterType;
            }
            ParameterTypes = types;
            _actionMethodInfo = value;
        }
    }

    internal Type InstanceType { get; set; }

    internal Type EventType { get; set; }

    internal int ActualRetryTimes => EnableRetry ? RetryTimes : 0;

    internal TaskInvokeDelegate? InvokeDelegate { get; private set; }

    internal bool IsEventHandler => FailureLevels == FailureLevels.Throw || FailureLevels == FailureLevels.ThrowAndCancel;

    internal Type[] ParameterTypes { get; private set; }

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

    public EventHandlerAttribute(int order, bool enableRetry, bool isCancel) : this(order, enableRetry, isCancel,
        enableRetry ? DefaultRetryCount : 0)
    {

    }

    public EventHandlerAttribute(int order, bool enableRetry, bool isCancel, int retryTimes) : this(order, FailureLevels.Throw, enableRetry,
        retryTimes, isCancel)
    {

    }

    public EventHandlerAttribute(int order, FailureLevels failureLevels, bool enableRetry) : this(order, failureLevels, enableRetry,
        enableRetry ? DefaultRetryCount : 0)
    {

    }

    public EventHandlerAttribute(int order, FailureLevels failureLevels, bool enableRetry, bool isCancel) : this(order, failureLevels,
        enableRetry, enableRetry ? DefaultRetryCount : 0, isCancel)
    {

    }

    public EventHandlerAttribute(int order, FailureLevels failureLevels, bool enableRetry, int retryTimes, bool isCancel = false)
    {
        Order = order;
        FailureLevels = failureLevels;
        EnableRetry = enableRetry;
        RetryTimes = enableRetry ? retryTimes : 0;
        IsCancel = isCancel;
    }

    internal void BuildExpression()
    {
        InvokeDelegate = InvokeBuilder.Build(ActionMethodInfo, InstanceType);
    }

    internal async Task ExecuteAction<TEvent>(IServiceProvider serviceProvider, TEvent @event) where TEvent : IEvent
    {
        await InvokeDelegate!.Invoke(serviceProvider.GetRequiredService(InstanceType), GetParameters(serviceProvider, @event));
    }

    private object?[] GetParameters<TEvent>(IServiceProvider serviceProvider, TEvent @event) where TEvent : IEvent
    {
        var parameters = new object?[ParameterTypes.Length];
        for (int index = 0; index < ParameterTypes.Length; index++)
        {
            if (ParameterTypes[index] == @event.GetType())
            {
                parameters[index] = @event;
            }
            else
            {
                parameters[index] = serviceProvider.GetService(ParameterTypes[index]);
            }
        }
        return parameters;
    }

    internal bool IsHandlerMissing(int maxCancelOrder) => FailureLevels == FailureLevels.ThrowAndCancel && Order < maxCancelOrder ||
        FailureLevels == FailureLevels.Throw && Order <= maxCancelOrder;
}
