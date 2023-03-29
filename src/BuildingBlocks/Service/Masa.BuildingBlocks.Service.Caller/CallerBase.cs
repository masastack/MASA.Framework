// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller;

public abstract class CallerBase
{
    public virtual string? Name { get; private set; } = null;

    /// <summary>
    /// Custom the current Caller request message handler
    /// default: null (Use the global request handler when no request handler is specified)
    /// </summary>
    protected virtual Func<IServiceProvider, IRequestMessage>? RequestMessageFactory => null;

    /// <summary>
    /// Custom the current Caller response message handler
    /// default: null (Use the global response handler when no response handler is specified)
    /// </summary>
    protected virtual Func<IServiceProvider, IResponseMessage>? ResponseMessageFactory => null;

    protected CallerBuilder CallerOptions { get; private set; } = default!;

    public ILogger? Logger { get; private set; }

    private ICaller? _caller;

    protected ICaller Caller => GetCaller();

    [Obsolete("CallerProvider has expired, please use Caller")]
    protected ICaller CallerProvider => Caller;

    public IServiceProvider? ServiceProvider { get; private set; }

    protected CallerBase() => ServiceProvider = null;

    protected CallerBase(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    protected virtual ICaller GetCaller()
        => _caller ??= ServiceProvider!.GetRequiredService<ICallerFactory>().Create(Name!);

    public abstract void UseCallerExtension();

    public void SetCallerOptions(CallerBuilder callerOptionsBuilder, string name)
    {
        CallerOptions = callerOptionsBuilder;
        Name = name;
    }

    internal void Initialize(IServiceProvider serviceProvider, Type type)
    {
        ServiceProvider ??= serviceProvider;
        Logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(type);
    }

    protected virtual void ConfigMasaCallerClient(MasaCallerClient callerClient)
    {
        callerClient.RequestMessageFactory = RequestMessageFactory;
        callerClient.ResponseMessageFactory = ResponseMessageFactory;
    }
}
