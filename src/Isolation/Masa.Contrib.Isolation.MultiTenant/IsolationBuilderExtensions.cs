namespace Masa.Contrib.Isolation.MultiTenant;

public static class IsolationBuilderExtensions
{
    public static IIsolationBuilder UseMultiTenant(this IIsolationBuilder isolationBuilder) => isolationBuilder.UseMultiTenant<Guid>();

    public static IIsolationBuilder UseMultiTenant<TKey>(this IIsolationBuilder isolationBuilder) where TKey : IComparable
    {
        isolationBuilder.Services.TryAddSingleton<IConvertProvider, ConvertProvider>();
        isolationBuilder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(ISaveChangesFilter), typeof(TenantSaveChangesFilter<TKey>), ServiceLifetime.Scoped));
        isolationBuilder.Services.TryAddScoped<TenantContext>();
        isolationBuilder.Services.TryAddScoped(typeof(ITenantContext), serviceProvider => serviceProvider.GetRequiredService<TenantContext>());
        isolationBuilder.Services.TryAddScoped(typeof(ITenantSetter), serviceProvider => serviceProvider.GetRequiredService<TenantContext>());
        return isolationBuilder;
    }
}
