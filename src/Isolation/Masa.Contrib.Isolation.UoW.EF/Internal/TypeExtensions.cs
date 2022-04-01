namespace Masa.Contrib.Isolation.UoW.EF.Internal;

internal static class TypeExtensions
{
    static bool IsConcrete(this Type type) => !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;

    public static bool IsGenericInterfaceAssignableFrom(this Type genericType, Type type) =>
        type.IsConcrete() &&
        type.GetInterfaces().Any(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == genericType);
}
