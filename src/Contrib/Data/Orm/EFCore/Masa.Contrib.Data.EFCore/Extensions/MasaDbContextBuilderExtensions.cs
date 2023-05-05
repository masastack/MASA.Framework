// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

[ExcludeFromCodeCoverage]
public static class MasaDbContextBuilderExtensions
{
    /// <summary>
    ///     <para>
    ///         Sets the model to be used for the context. If the model is set, then <see cref="DbContext.OnModelCreating(ModelBuilder)" />
    ///         will not be run.
    ///     </para>
    ///     <para>
    ///         If setting an externally created model <see cref="ModelBuilder.FinalizeModel()" /> should be called first.
    ///     </para>
    /// </summary>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="model">The model to be used.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder UseModel(
        this MasaDbContextBuilder masaDbContextBuilder,
        IModel model)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.UseModel(model));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Sets the <see cref="ILoggerFactory" /> that will be used to create <see cref="ILogger" /> instances
    ///         for logging done by this context.
    ///     </para>
    ///     <para>
    ///         There is no need to call this method when using one of the 'AddDbContext' methods, including 'AddDbContextPool'.
    ///         These methods ensure that the <see cref="ILoggerFactory" /> used by EF is obtained from the application service provider.
    ///     </para>
    ///     <para>
    ///         This method cannot be used if the application is setting the internal service provider
    ///         through a call to <see cref="UseInternalServiceProvider" />. In this case, the <see cref="ILoggerFactory" />
    ///         should be configured directly in that service provider.
    ///     </para>
    /// </summary>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="loggerFactory">The logger factory to be used.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder UseLoggerFactory(
        this MasaDbContextBuilder masaDbContextBuilder,
        ILoggerFactory? loggerFactory)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.UseLoggerFactory(loggerFactory));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Logs using the supplied action. For example, use <c>optionsBuilder.LogTo(Console.WriteLine)</c> to
    ///         log to the console.
    ///     </para>
    ///     <para>
    ///         This overload allows the minimum level of logging and the log formatting to be controlled.
    ///         Use the
    ///         <see
    ///             cref="LogTo(Action{string},System.Collections.Generic.IEnumerable{Microsoft.Extensions.Logging.EventId},LogLevel,DbContextLoggerOptions?)" />
    ///         overload to log only specific events.
    ///         Use the <see cref="LogTo(Action{string},IEnumerable{string},LogLevel,DbContextLoggerOptions?)" />
    ///         overload to log only events in specific categories.
    ///         Use the <see cref="LogTo(Action{string},Func{EventId,LogLevel,bool},DbContextLoggerOptions?)" />
    ///         overload to use a custom filter for events.
    ///         Use the <see cref="LogTo(Func{EventId,LogLevel,bool},Action{EventData})" /> overload to log to a fully custom logger.
    ///     </para>
    /// </summary>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="action">Delegate called when there is a message to log.</param>
    /// <param name="minimumLevel">The minimum level of logging event to log. Defaults to <see cref="LogLevel.Debug" /></param>
    /// <param name="options">
    ///     Formatting options for log messages. Passing null (the default) means use <see cref="DbContextLoggerOptions.DefaultWithLocalTime" />
    /// </param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder LogTo(
        this MasaDbContextBuilder masaDbContextBuilder,
        Action<string> action,
        LogLevel minimumLevel = LogLevel.Debug,
        DbContextLoggerOptions? options = null)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.LogTo(action, minimumLevel, options));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Logs the specified events using the supplied action. For example, use
    ///         <c>optionsBuilder.LogTo(Console.WriteLine, new[] { CoreEventId.ContextInitialized })</c> to log the
    ///         <see cref="CoreEventId.ContextInitialized" /> event to the console.
    ///     </para>
    ///     <para>
    ///         Use the <see cref="LogTo(Action{string},LogLevel,DbContextLoggerOptions?)" /> overload for default logging of
    ///         all events.
    ///         Use the <see cref="LogTo(Action{string},IEnumerable{string},LogLevel,DbContextLoggerOptions?)" />
    ///         overload to log only events in specific categories.
    ///         Use the <see cref="LogTo(Action{string},Func{EventId,LogLevel,bool},DbContextLoggerOptions?)" />
    ///         overload to use a custom filter for events.
    ///         Use the <see cref="LogTo(Func{EventId,LogLevel,bool},Action{EventData})" /> overload to log to a fully custom logger.
    ///     </para>
    /// </summary>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="action">Delegate called when there is a message to log.</param>
    /// <param name="events">The <see cref="EventId" /> of each event to log.</param>
    /// <param name="minimumLevel">The minimum level of logging event to log. Defaults to <see cref="LogLevel.Debug" /></param>
    /// <param name="options">
    ///     Formatting options for log messages. Passing null (the default) means use <see cref="DbContextLoggerOptions.DefaultWithLocalTime" />
    /// </param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder LogTo(
        this MasaDbContextBuilder masaDbContextBuilder,
        Action<string> action,
        IEnumerable<EventId> events,
        LogLevel minimumLevel = LogLevel.Debug,
        DbContextLoggerOptions? options = null)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.LogTo(action, events, minimumLevel, options));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Logs all events in the specified categories using the supplied action. For example, use
    ///         <c>optionsBuilder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Infrastructure.Name })</c> to log all
    ///         events in the <see cref="DbLoggerCategory.Infrastructure" /> category.
    ///     </para>
    ///     <para>
    ///         Use the <see cref="LogTo(Action{string},LogLevel,DbContextLoggerOptions?)" /> overload for default logging of
    ///         all events.
    ///         Use the <see cref="LogTo(Action{string},IEnumerable{EventId},LogLevel,DbContextLoggerOptions?)" />
    ///         overload to log only specific events.
    ///         Use the <see cref="LogTo(Action{string},Func{EventId,LogLevel,bool},DbContextLoggerOptions?)" />
    ///         overload to use a custom filter for events.
    ///         Use the <see cref="LogTo(Func{EventId,LogLevel,bool},Action{EventData})" /> overload to log to a fully custom logger.
    ///     </para>
    /// </summary>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="action">Delegate called when there is a message to log.</param>
    /// <param name="categories">The <see cref="DbLoggerCategory" /> of each event to log.</param>
    /// <param name="minimumLevel">The minimum level of logging event to log. Defaults to <see cref="LogLevel.Debug" /></param>
    /// <param name="options">
    ///     Formatting options for log messages. Passing null (the default) means use <see cref="DbContextLoggerOptions.DefaultWithLocalTime" />
    /// </param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder LogTo(
        this MasaDbContextBuilder masaDbContextBuilder,
        Action<string> action,
        IEnumerable<string> categories,
        LogLevel minimumLevel = LogLevel.Debug,
        DbContextLoggerOptions? options = null)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.LogTo(action, categories, minimumLevel, options));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Logs events filtered by a supplied custom filter delegate. The filter should return true to
    ///         log a message, or false to filter it out of the log.
    ///     </para>
    ///     <para>
    ///         Use the <see cref="LogTo(Action{string},LogLevel,DbContextLoggerOptions?)" /> overload for default logging of
    ///         all events.
    ///         Use the <see cref="LogTo(Action{string},IEnumerable{EventId},LogLevel,DbContextLoggerOptions?)" />
    ///         Use the <see cref="LogTo(Action{string},IEnumerable{string},LogLevel,DbContextLoggerOptions?)" />
    ///         overload to log only events in specific categories.
    ///         Use the <see cref="LogTo(Func{EventId,LogLevel,bool},Action{EventData})" /> overload to log to a fully custom logger.
    ///     </para>
    /// </summary>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="action">Delegate called when there is a message to log.</param>
    /// <param name="filter">Delegate that returns true to log the message or false to ignore it.</param>
    /// <param name="options">
    ///     Formatting options for log messages. Passing null (the default) means use <see cref="DbContextLoggerOptions.DefaultWithLocalTime" />
    /// </param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder LogTo(
        this MasaDbContextBuilder masaDbContextBuilder,
        Action<string> action,
        Func<EventId, LogLevel, bool> filter,
        DbContextLoggerOptions? options = null)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.LogTo(action, filter, options));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Logs events to a custom logger delegate filtered by a custom filter delegate. The filter should return true to
    ///         log a message, or false to filter it out of the log.
    ///     </para>
    ///     <para>
    ///         Use the <see cref="LogTo(Action{string},LogLevel,DbContextLoggerOptions?)" /> overload for default logging of
    ///         all events.
    ///         Use the <see cref="LogTo(Action{string},IEnumerable{EventId},LogLevel,DbContextLoggerOptions?)" />
    ///         Use the <see cref="LogTo(Action{string},IEnumerable{string},LogLevel,DbContextLoggerOptions?)" />
    ///         overload to log only events in specific categories.
    ///         Use the <see cref="LogTo(Action{string},Func{EventId,LogLevel,bool},DbContextLoggerOptions?)" />
    ///         overload to use a custom filter for events.
    ///     </para>
    /// </summary>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="filter">Delegate that returns true to log the message or false to ignore it.</param>
    /// <param name="logger">Delegate called when there is a message to log.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    // Filter comes first, logger second, otherwise it's hard to get the correct overload to resolve
    public static MasaDbContextBuilder LogTo(
        this MasaDbContextBuilder masaDbContextBuilder,
        Func<EventId, LogLevel, bool> filter,
        Action<EventData> logger)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.LogTo(filter, logger));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Disables concurrency detection, which detects many cases of erroneous concurrent usage of a <see cref="DbContext" />
    ///         instance and causes an informative exception to be thrown. This provides a minor performance improvement, but if a
    ///         <see cref="DbContext" /> instance is used concurrently, the behavior will be undefined and the program may fail in
    ///         unpredictable ways.
    ///     </para>
    ///     <para>
    ///         Only disable concurrency detection after confirming that the performance gains are considerable, and the application has
    ///         been thoroughly tested against concurrency bugs.
    ///     </para>
    ///     <para>
    ///         Note that if the application is setting the internal service provider through a call to
    ///         <see cref="UseInternalServiceProvider" />, then this option must configured the same way
    ///         for all uses of that service provider. Consider instead not calling <see cref="UseInternalServiceProvider" />
    ///         so that EF will manage the service providers and can create new instances as required.
    ///     </para>
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder EnableThreadSafetyChecks(
        this MasaDbContextBuilder masaDbContextBuilder,
        bool enableChecks = true)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.EnableThreadSafetyChecks(enableChecks));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Enables detailed errors when handling of data value exceptions that occur during processing of store query results. Such errors
    ///         most often occur due to misconfiguration of entity properties. E.g. If a property is configured to be of type
    ///         'int', but the underlying data in the store is actually of type 'string', then an exception will be generated
    ///         at runtime during processing of the data value. When this option is enabled and a data error is encountered, the
    ///         generated exception will include details of the specific entity property that generated the error.
    ///     </para>
    ///     <para>
    ///         Enabling this option incurs a small performance overhead during query execution.
    ///     </para>
    ///     <para>
    ///         Note that if the application is setting the internal service provider through a call to
    ///         <see cref="UseInternalServiceProvider" />, then this option must configured the same way
    ///         for all uses of that service provider. Consider instead not calling <see cref="UseInternalServiceProvider" />
    ///         so that EF will manage the service providers and can create new instances as required.
    ///     </para>
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder EnableDetailedErrors(
        this MasaDbContextBuilder masaDbContextBuilder,
        bool detailedErrorsEnabled = true)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.EnableDetailedErrors(detailedErrorsEnabled));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Sets the <see cref="IMemoryCache" /> to be used for query caching by this context.
    ///     </para>
    ///     <para>
    ///         Note that changing the memory cache can cause EF to build a new internal service provider, which
    ///         may cause issues with performance. Generally it is expected that no more than one or two different
    ///         instances will be used for a given application.
    ///     </para>
    ///     <para>
    ///         This method cannot be used if the application is setting the internal service provider
    ///         through a call to <see cref="UseInternalServiceProvider" />. In this case, the <see cref="IMemoryCache" />
    ///         should be configured directly in that service provider.
    ///     </para>
    /// </summary>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="memoryCache">The memory cache to be used.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder UseMemoryCache(
        this MasaDbContextBuilder masaDbContextBuilder,
        IMemoryCache? memoryCache)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.UseMemoryCache(memoryCache));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Sets the <see cref="IServiceProvider" /> that the context should resolve all of its services from. EF will
    ///         create and manage a service provider if none is specified.
    ///     </para>
    ///     <para>
    ///         The service provider must contain all the services required by Entity Framework (and the database being
    ///         used). The Entity Framework services can be registered using an extension method on <see cref="IServiceCollection" />.
    ///         For example, the Microsoft SQL Server provider includes an AddEntityFrameworkSqlServer() method to add
    ///         the required services.
    ///     </para>
    ///     <para>
    ///         If the <see cref="IServiceProvider" /> has a <see cref="DbContextOptions" /> or
    ///         <see cref="DbContextOptions{TContext}" /> registered, then this will be used as the options for
    ///         this context instance.
    ///     </para>
    /// </summary>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="serviceProvider">The service provider to be used.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder UseInternalServiceProvider(
        this MasaDbContextBuilder masaDbContextBuilder,
        IServiceProvider? serviceProvider)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.UseInternalServiceProvider(serviceProvider));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     Sets the <see cref="IServiceProvider" /> from which application services will be obtained. This
    ///     is done automatically when using 'AddDbContext' or 'AddDbContextPool',
    ///     so it is rare that this method needs to be called.
    /// </summary>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="serviceProvider">The service provider to be used.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder UseApplicationServiceProvider(
        this MasaDbContextBuilder masaDbContextBuilder,
        IServiceProvider? serviceProvider)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.UseApplicationServiceProvider(serviceProvider));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Enables application data to be included in exception messages, logging, etc. This can include the
    ///         values assigned to properties of your entity instances, parameter values for commands being sent
    ///         to the database, and other such data. You should only enable this flag if you have the appropriate
    ///         security measures in place based on the sensitivity of this data.
    ///     </para>
    ///     <para>
    ///         Note that if the application is setting the internal service provider through a call to
    ///         <see cref="UseInternalServiceProvider" />, then this option must configured the same way
    ///         for all uses of that service provider. Consider instead not calling <see cref="UseInternalServiceProvider" />
    ///         so that EF will manage the service providers and can create new instances as required.
    ///     </para>
    /// </summary>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="sensitiveDataLoggingEnabled">If <see langword="true" />, then sensitive data is logged.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder EnableSensitiveDataLogging(
        this MasaDbContextBuilder masaDbContextBuilder,
        bool sensitiveDataLoggingEnabled = true)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.EnableSensitiveDataLogging(sensitiveDataLoggingEnabled));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Enables or disables caching of internal service providers. Disabling caching can
    ///         massively impact performance and should only be used in testing scenarios that
    ///         build many service providers for test isolation.
    ///     </para>
    ///     <para>
    ///         Note that if the application is setting the internal service provider through a call to
    ///         <see cref="UseInternalServiceProvider" />, then setting this option wil have no effect.
    ///     </para>
    /// </summary>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="cacheServiceProvider">If <see langword="true" />, then the internal service provider is cached.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder EnableServiceProviderCaching(
        this MasaDbContextBuilder masaDbContextBuilder,
        bool cacheServiceProvider = true)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.EnableServiceProviderCaching(cacheServiceProvider));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Sets the tracking behavior for LINQ queries run against the context. Disabling change tracking
    ///         is useful for read-only scenarios because it avoids the overhead of setting up change tracking for each
    ///         entity instance. You should not disable change tracking if you want to manipulate entity instances and
    ///         persist those changes to the database using <see cref="DbContext.SaveChanges()" />.
    ///     </para>
    ///     <para>
    ///         This method sets the default behavior for all contexts created with these options, but you can override this
    ///         behavior for a context instance using <see cref="ChangeTracker.QueryTrackingBehavior" /> or on individual
    ///         queries using the <see cref="EntityFrameworkQueryableExtensions.AsNoTracking{TEntity}(IQueryable{TEntity})" />
    ///         and <see cref="EntityFrameworkQueryableExtensions.AsTracking{TEntity}(IQueryable{TEntity})" /> methods.
    ///     </para>
    ///     <para>
    ///         The default value is <see cref="QueryTrackingBehavior.TrackAll" />. This means
    ///         the change tracker will keep track of changes for all entities that are returned from a LINQ query.
    ///     </para>
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder UseQueryTrackingBehavior(
        this MasaDbContextBuilder masaDbContextBuilder,
        QueryTrackingBehavior queryTrackingBehavior)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.UseQueryTrackingBehavior(queryTrackingBehavior));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Configures the runtime behavior of warnings generated by Entity Framework. You can set a default
    ///         behavior and behaviors for each warning type.
    ///     </para>
    ///     <para>
    ///         Note that changing this configuration can cause EF to build a new internal service provider, which
    ///         may cause issues with performance. Generally it is expected that no more than one or two different
    ///         configurations will be used for a given application.
    ///     </para>
    ///     <para>
    ///         Note that if the application is setting the internal service provider through a call to
    ///         <see cref="UseInternalServiceProvider" />, then this option must configured the same way
    ///         for all uses of that service provider. Consider instead not calling <see cref="UseInternalServiceProvider" />
    ///         so that EF will manage the service providers and can create new instances as required.
    ///     </para>
    /// </summary>
    /// <example>
    ///     <code>
    ///  optionsBuilder.ConfigureWarnings(warnings =>
    ///      warnings.Default(WarningBehavior.Ignore)
    ///          .Log(CoreEventId.IncludeIgnoredWarning, CoreEventId.ModelValidationWarning)
    ///          .Throw(RelationalEventId.BoolWithDefaultWarning));
    ///      </code>
    /// </example>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="warningsConfigurationBuilderAction">
    ///     An action to configure the warning behavior.
    /// </param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder ConfigureWarnings(
        this MasaDbContextBuilder masaDbContextBuilder,
        Action<WarningsConfigurationBuilder> warningsConfigurationBuilderAction)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.ConfigureWarnings(warningsConfigurationBuilderAction));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Replaces all internal Entity Framework implementations of a service contract with a different
    ///         implementation.
    ///     </para>
    ///     <para>
    ///         This method can only be used when EF is building and managing its internal service provider.
    ///         If the service provider is being built externally and passed to
    ///         <see cref="UseInternalServiceProvider" />, then replacement services should be configured on
    ///         that service provider before it is passed to EF.
    ///     </para>
    ///     <para>
    ///         The replacement service gets the same scope as the EF service that it is replacing.
    ///     </para>
    /// </summary>
    /// <typeparam name="TService">The type (usually an interface) that defines the contract of the service to replace.</typeparam>
    /// <typeparam name="TImplementation">The new implementation type for the service.</typeparam>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder ReplaceService<TService, TImplementation>(this MasaDbContextBuilder masaDbContextBuilder)
        where TImplementation : TService
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.ReplaceService<TService, TImplementation>());
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Replaces the internal Entity Framework implementation of a specific implementation of a service contract
    ///         with a different implementation.
    ///     </para>
    ///     <para>
    ///         This method is useful for replacing a single instance of services that can be legitimately registered
    ///         multiple times in the EF internal service provider.
    ///     </para>
    ///     <para>
    ///         This method can only be used when EF is building and managing its internal service provider.
    ///         If the service provider is being built externally and passed to
    ///         <see cref="UseInternalServiceProvider" />, then replacement services should be configured on
    ///         that service provider before it is passed to EF.
    ///     </para>
    ///     <para>
    ///         The replacement service gets the same scope as the EF service that it is replacing.
    ///     </para>
    /// </summary>
    /// <typeparam name="TService">The type (usually an interface) that defines the contract of the service to replace.</typeparam>
    /// <typeparam name="TCurrentImplementation">The current implementation type for the service.</typeparam>
    /// <typeparam name="TNewImplementation">The new implementation type for the service.</typeparam>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder ReplaceService<TService, TCurrentImplementation, TNewImplementation>(
        this MasaDbContextBuilder masaDbContextBuilder)
        where TCurrentImplementation : TService
        where TNewImplementation : TService
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.ReplaceService<TService, TCurrentImplementation, TNewImplementation>());
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Adds <see cref="IInterceptor" /> instances to those registered on the context.
    ///     </para>
    ///     <para>
    ///         Interceptors can be used to view, change, or suppress operations taken by Entity Framework.
    ///         See the specific implementations of <see cref="IInterceptor" /> for details. For example, 'IDbCommandInterceptor'.
    ///     </para>
    ///     <para>
    ///         A single interceptor instance can implement multiple different interceptor interfaces. It will be registered as
    ///         an interceptor for all interfaces that it implements.
    ///     </para>
    ///     <para>
    ///         Extensions can also register multiple <see cref="IInterceptor" />s in the internal service provider.
    ///         If both injected and application interceptors are found, then the injected interceptors are run in the
    ///         order that they are resolved from the service provider, and then the application interceptors are run
    ///         in the order that they were added to the context.
    ///     </para>
    ///     <para>
    ///         Calling this method multiple times will result in all interceptors in every call being added to the context.
    ///         Interceptors added in a previous call are not overridden by interceptors added in a later call.
    ///     </para>
    /// </summary>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="interceptors">The interceptors to add.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder AddInterceptors(
        this MasaDbContextBuilder masaDbContextBuilder,
        IEnumerable<IInterceptor> interceptors)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.AddInterceptors(interceptors));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Adds <see cref="IInterceptor" /> instances to those registered on the context.
    ///     </para>
    ///     <para>
    ///         Interceptors can be used to view, change, or suppress operations taken by Entity Framework.
    ///         See the specific implementations of <see cref="IInterceptor" /> for details. For example, 'IDbCommandInterceptor'.
    ///     </para>
    ///     <para>
    ///         Extensions can also register multiple <see cref="IInterceptor" />s in the internal service provider.
    ///         If both injected and application interceptors are found, then the injected interceptors are run in the
    ///         order that they are resolved from the service provider, and then the application interceptors are run
    ///         in the order that they were added to the context.
    ///     </para>
    ///     <para>
    ///         Calling this method multiple times will result in all interceptors in every call being added to the context.
    ///         Interceptors added in a previous call are not overridden by interceptors added in a later call.
    ///     </para>
    /// </summary>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="interceptors">The interceptors to add.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder AddInterceptors(
        this MasaDbContextBuilder masaDbContextBuilder,
        params IInterceptor[] interceptors)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.AddInterceptors(interceptors));
        return masaDbContextBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Configures how long EF Core will cache logging configuration in certain high-performance paths. This makes
    ///         EF Core skip potentially costly logging checks, but means that runtime logging changes (e.g. registering a
    ///         new <see cref="DiagnosticListener" /> may not be taken into account right away).
    ///     </para>
    ///     <para>
    ///         Defaults to one second.
    ///     </para>
    /// </summary>
    /// <param name="masaDbContextBuilder"></param>
    /// <param name="timeSpan">The maximum time period over which to skip logging checks before checking again.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static MasaDbContextBuilder ConfigureLoggingCacheTime(
        this MasaDbContextBuilder masaDbContextBuilder,
        TimeSpan timeSpan)
    {
        masaDbContextBuilder.DbContextOptionsBuilders.Add(builder => builder.ConfigureLoggingCacheTime(timeSpan));
        return masaDbContextBuilder;
    }
}
