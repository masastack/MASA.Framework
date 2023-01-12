// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

/// <summary>
/// Caller dependency orchestration
/// </summary>
internal static class CallerDependExtensions
{
    /// <summary>
    /// Caller dependency orchestration
    /// </summary>
    /// <param name="callerTypes">All Callers that inherit CallerBase</param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public static List<Type> Arrangement(this List<Type> callerTypes)
    {
        List<Type> types = GetCallerByNotDependCaller(callerTypes);
        if (types.Count == 0)
            throw new UserFriendlyException(ErrorMessages.CIRCULAR_DEPENDENCY);

        return callerTypes.CallersArrangement(types, 1);
    }

    private static List<Type> CallersArrangement(this List<Type> allTypes, List<Type> existTypes, int executeTimes)
    {
        List<Type> types = existTypes;
        var dependCallerTypes = allTypes.Except(existTypes);
        foreach (var type in dependCallerTypes)
        {
            var constructorInfo = type.GetConstructors().MaxBy(con => con.GetParameters().Length)!;
            bool isExist = true;
            foreach (var parameter in constructorInfo.GetParameters())
            {
                var parameterType = parameter.ParameterType;
                if (typeof(CallerBase).IsAssignableFrom(parameterType) && !types.Contains(parameterType))
                {
                    isExist = false;
                }
            }
            if (isExist)
                types.Add(type);
        }

        if (types.Count != allTypes.Count)
        {
            if (executeTimes >= allTypes.Count)
                throw new UserFriendlyException(ErrorMessages.CIRCULAR_DEPENDENCY);

            return CallersArrangement(allTypes, types, executeTimes + 1);
        }
        return types;
    }

    /// <summary>
    /// Get a Caller object that does not depend on other Callers
    /// </summary>
    /// <param name="callerTypes"></param>
    /// <returns></returns>
    private static List<Type> GetCallerByNotDependCaller(this List<Type> callerTypes)
    {
        List<Type> types = new();
        callerTypes.ForEach(type =>
        {
            if (!type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).IsDependCaller())
                types.Add(type);
        });
        return types;
    }

    private static bool IsDependCaller(this ConstructorInfo[] constructorInfos)
    {
        var constructorInfo = constructorInfos.MaxBy(constructorInfo => constructorInfo.GetParameters().Length)!;
        return constructorInfo.IsDependCaller();
    }

    private static bool IsDependCaller(this ConstructorInfo constructorInfo)
    {
        foreach (var parameter in constructorInfo.GetParameters())
        {
            if (typeof(CallerBase).IsAssignableFrom(parameter.ParameterType))
            {
                return true;
            }
        }
        return false;
    }
}
