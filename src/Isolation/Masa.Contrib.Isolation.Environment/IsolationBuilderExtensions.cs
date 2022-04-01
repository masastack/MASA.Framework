namespace Masa.Contrib.Isolation.Environment;

public static class IsolationBuilderExtensions
{
    public static IIsolationBuilder UseEnvironment(this IIsolationBuilder isolationBuilder)
    {
        isolationBuilder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(ISaveChangesFilter), typeof(EnvironmentSaveChangesFilter), ServiceLifetime.Scoped));
        isolationBuilder.Services.TryAddScoped<EnvironmentContext>();
        isolationBuilder.Services.TryAddScoped(typeof(IEnvironmentContext), serviceProvider => serviceProvider.GetRequiredService<EnvironmentContext>());
        isolationBuilder.Services.TryAddScoped(typeof(IEnvironmentSetter), serviceProvider => serviceProvider.GetRequiredService<EnvironmentContext>());
        return isolationBuilder;
    }
}
