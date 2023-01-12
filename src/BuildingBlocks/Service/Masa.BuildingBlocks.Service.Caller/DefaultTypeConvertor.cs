// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller;

public class DefaultTypeConvertor : ITypeConvertor
{
    private static readonly ConcurrentDictionary<Type, List<PropertyInfoMember>> Dictionary = new();

    protected readonly List<Type> NotNeedSerializeTypes = new()
    {
        typeof(String),
        typeof(Guid),
        typeof(DateTime),
        typeof(Decimal),
        typeof(Guid?),
        typeof(DateTime?),
        typeof(Decimal?)
    };

    /// <summary>
    /// Convert custom object to dictionary
    /// </summary>
    /// <param name="request"></param>
    /// <typeparam name="TRequest">Support classes, anonymous objects</typeparam>
    /// <returns></returns>
    [Obsolete("Use ConvertToKeyValuePairs instead")]
    public Dictionary<string, string> ConvertToDictionary<TRequest>(TRequest? request) where TRequest : class
        => new(ConvertToKeyValuePairs(request));

    /// <summary>
    /// Convert custom object to dictionary
    /// </summary>
    /// <param name="request"></param>
    /// <typeparam name="TRequest">Support classes, anonymous objects</typeparam>
    /// <returns></returns>
    public IEnumerable<KeyValuePair<string, string>> ConvertToKeyValuePairs<TRequest>(TRequest? request) where TRequest : class
    {
        if (request is null)
            return Array.Empty<KeyValuePair<string, string>>();

        if (request is Dictionary<string, string> response)
            return response;

        if (request is IEnumerable<KeyValuePair<string, string>> keyValuePairs)
            return keyValuePairs;

        var requestType = request.GetType();
        if (!Dictionary.TryGetValue(requestType, out List<PropertyInfoMember>? members))
        {
            members = GetMembers(request.GetType().GetProperties());
            Dictionary.TryAdd(requestType, members);
        }
        List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
        foreach (var member in members)
        {
            if (member.TryGetValue(request, out string value))
                data.Add(new KeyValuePair<string, string>(member.Name, value));
        }
        return data;
    }

    private List<PropertyInfoMember> GetMembers(PropertyInfo[] properties)
    {
        List<PropertyInfoMember> members = new();
        foreach (var property in properties)
        {
            if (IsSkip(property)) continue;

            string name = GetPropertyName(property);

            members.Add(new PropertyInfoMember(property, name, IsNeedSerialize(property)));
        }
        return members;
    }

    protected virtual bool IsSkip(PropertyInfo property)
        => !property.CanRead ||
            !property.PropertyType.IsPublic ||
            property.CustomAttributes.Any(attr => attr.AttributeType == typeof(JsonIgnoreAttribute));

    protected virtual string GetPropertyName(PropertyInfo property)
    {
        if (property.CustomAttributes.Any(attr => attr.AttributeType == typeof(JsonPropertyNameAttribute)))
        {
            var customAttributeData =
                property.CustomAttributes.FirstOrDefault(attr => attr.AttributeType == typeof(JsonPropertyNameAttribute))!;
            var customAttribute = customAttributeData.ConstructorArguments.FirstOrDefault();
            return customAttribute.Value?.ToString() ??
                throw new NotSupportedException(
                    $"Parameter name: {property.Name}, But the JsonPropertyNameAttribute assignment name is empty");
        }
        return property.Name;
    }

    protected virtual bool IsNeedSerialize(PropertyInfo property)
        => !property.PropertyType.IsPrimitive && !NotNeedSerializeTypes.Contains(property.PropertyType);
}
