// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore;

public class MasaDbContextOptions<TContext> : MasaDbContextOptions
    where TContext : DbContext
{
    private readonly DbContextOptions _originOptions;

    public MasaDbContextOptions(
        IServiceProvider? serviceProvider,
        DbContextOptions originOptions,
        bool enableSoftDelete) : base(serviceProvider, enableSoftDelete) => _originOptions = originOptions;

    private IEnumerable<IModelCreatingProvider>? _modelCreatingProviders;

    /// <summary>
    /// Can be used to filter data
    /// </summary>
    public override IEnumerable<IModelCreatingProvider> ModelCreatingProviders
        => _modelCreatingProviders ??= ServiceProvider?.GetServices<IModelCreatingProvider>() ?? new List<IModelCreatingProvider>();

    private IEnumerable<ISaveChangesFilter>? _saveChangesFilters;

    /// <summary>
    /// Can be used to intercept SaveChanges(Async) method
    /// </summary>
    public override IEnumerable<ISaveChangesFilter> SaveChangesFilters
        => _saveChangesFilters ??= ServiceProvider?.GetServices<ISaveChangesFilter>() ?? new List<ISaveChangesFilter>();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override Type ContextType => _originOptions.ContextType;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool IsFrozen => _originOptions.IsFrozen;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override IEnumerable<IDbContextOptionsExtension> Extensions => _originOptions.Extensions;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="TExtension"></typeparam>
    /// <param name="extension"></param>
    /// <returns></returns>
    public override DbContextOptions WithExtension<TExtension>(TExtension extension)
        => _originOptions.WithExtension(extension);

    public override TExtension? FindExtension<TExtension>() where TExtension : class
        => _originOptions.FindExtension<TExtension>();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Freeze() => _originOptions.Freeze();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="TExtension"></typeparam>
    /// <returns></returns>
    public override TExtension GetExtension<TExtension>()
        => _originOptions.GetExtension<TExtension>();
}
