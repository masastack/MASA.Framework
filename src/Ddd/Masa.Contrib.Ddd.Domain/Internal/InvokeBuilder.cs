namespace Masa.Contrib.Ddd.Domain.Internal;

internal class InvokeBuilder
{
    internal delegate Task TaskInvokeDelegate(object target, params object[] parameters);

    internal static TaskInvokeDelegate Build(Type targetType, string methodName, Type parameterType)
    {
        var targetParameter = Expression.Parameter(typeof(object), "target");
        var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

        var valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(0));
        var valueCast = Expression.Convert(valueObj, parameterType);
        var instanceCast = Expression.Convert(targetParameter, targetType);
        var methodCall = Expression.Call(instanceCast, methodName, new[] { parameterType }, valueCast);

        var castMethodCall = Expression.Convert(methodCall, typeof(Task));
        var lambda = Expression.Lambda<TaskInvokeDelegate>(castMethodCall, targetParameter, parametersParameter);
        return lambda.Compile();
    }
}
