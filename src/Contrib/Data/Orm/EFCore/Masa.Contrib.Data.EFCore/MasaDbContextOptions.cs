// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public abstract class MasaDbContextOptions : DbContextOptions
{
    public readonly IServiceProvider? ServiceProvider;

    public abstract IEnumerable<IModelCreatingProvider> ModelCreatingProviders { get; }

    /// <summary>
    /// Can be used to intercept SaveChanges(Async) method
    /// </summary>
    public abstract IEnumerable<ISaveChangesFilter> SaveChangesFilters { get; }

    internal virtual bool IsInitialize => ExtensionsMap.Count > 0;

    public bool EnableSoftDelete { get; }

    internal Type DbContextType { get; }

    protected readonly DbContextOptions OriginOptions;

    private protected MasaDbContextOptions(
        IServiceProvider? serviceProvider,
        bool enableSoftDelete,
        Type dbContextType,
        DbContextOptions originOptions)
    {
        ServiceProvider = serviceProvider;
        EnableSoftDelete = enableSoftDelete;
        DbContextType = dbContextType;

        OriginOptions = originOptions;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override Type ContextType => OriginOptions.ContextType;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool IsFrozen => OriginOptions.IsFrozen;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override IEnumerable<IDbContextOptionsExtension> Extensions => OriginOptions.Extensions;

#pragma warning disable CS8603
#pragma warning disable S1135
    /// <summary>
    /// todo: Subsequent change to lambda tree to optimize performance
    /// </summary>
    protected override ImmutableSortedDictionary<Type, (IDbContextOptionsExtension Extension, int Ordinal)> ExtensionsMap
        =>  typeof(DbContextOptions).GetProperty("ExtensionsMap", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?
            .GetValue(OriginOptions) as ImmutableSortedDictionary<Type, (IDbContextOptionsExtension Extension, int Ordinal)>;
#pragma warning restore S1135
#pragma warning restore CS8603

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="TExtension"></typeparam>
    /// <param name="extension"></param>
    /// <returns></returns>
    public override DbContextOptions WithExtension<TExtension>(TExtension extension)
        => OriginOptions.WithExtension(extension);

    public override TExtension? FindExtension<TExtension>() where TExtension : class
        => OriginOptions.FindExtension<TExtension>();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Freeze() => OriginOptions.Freeze();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="TExtension"></typeparam>
    /// <returns></returns>
    public override TExtension GetExtension<TExtension>()
        => OriginOptions.GetExtension<TExtension>();

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        foreach (var dbContextOptionsExtension in ExtensionsMap)
        {
            hashCode.Add(dbContextOptionsExtension.Key);
            hashCode.Add(dbContextOptionsExtension.Value.Extension.Info.GetServiceProviderHashCode());
        }

        return hashCode.ToHashCode();
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj)
            || (obj is DbContextOptions otherOptions && Equals(otherOptions));
}
