namespace Microsoft.EntityFrameworkCore;

public static class MasaDynamicsCrmDbContextBuilderExtensions
{
    public static IMasaDbContextBuilder UseDynamicsCrmFilter<TDbContext>(
        this IMasaDbContextBuilder masaDbContextBuilder,
        Action<FilterOptions>? options = null) where TDbContext : MasaDynamicsCrmDbContext, IMasaDbContext
        => masaDbContextBuilder.UseFilter(options).UseDynamicsCrmFilterCore<TDbContext>();

    private static IMasaDbContextBuilder UseDynamicsCrmFilterCore<TDbContext>(this IMasaDbContextBuilder masaDbContextBuilder) where TDbContext : MasaDynamicsCrmDbContext, IMasaDbContext
    {
        masaDbContextBuilder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContext>),
        typeof(DynamicsCrmSaveChangeFilter<TDbContext, Guid>), ServiceLifetime.Scoped));

        masaDbContextBuilder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContext>),
        typeof(DynamicsCrmSoftDeleteSaveChangesFilter<TDbContext, Guid>), ServiceLifetime.Scoped));

        return masaDbContextBuilder;
    }
}
