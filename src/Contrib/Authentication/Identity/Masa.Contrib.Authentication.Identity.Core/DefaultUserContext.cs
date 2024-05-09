// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Collections;

namespace Masa.Contrib.Authentication.Identity;

internal class DefaultUserContext : UserContext
{
    private readonly IOptionsMonitor<IdentityClaimOptions> _optionsMonitor;
    private static readonly MemoryCache<Type, CustomizeModelRelation> ModelRelationCache = new();

    private ClaimsPrincipal? ClaimsPrincipal { get; set; }

    public DefaultUserContext(
        ITypeConvertProvider typeConvertProvider,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IOptionsMonitor<IdentityClaimOptions> optionsMonitor)
        : base(typeConvertProvider)
    {
        _optionsMonitor = optionsMonitor;
        _optionsMonitor.CurrentValue.Initialize();
        ClaimsPrincipal = currentPrincipalAccessor.GetCurrentPrincipal();
    }

    protected override object? GetUser(Type userType)
    {
        var userClaimType = _optionsMonitor.CurrentValue.GetClaimType(nameof(_optionsMonitor.CurrentValue.UserId))!;
        var userId = ClaimsPrincipal?.FindClaimValue(userClaimType);
        if (userId == null)
            return null;

        var modelRelation = ModelRelationCache.GetOrAdd(userType, (type) =>
        {
            var constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                                  .FirstOrDefault(c => c.GetParameters().Length == 0) ??
                              throw new InvalidOperationException($"[{type.Name}] has a parameterless constructor");
            return new CustomizeModelRelation(
                InstanceBuilder.CreateInstanceDelegate(constructor),
                InstanceBuilder.GetPropertyAndMethodInfoRelations(type));
        });
        var userModel = modelRelation.Func.Invoke(Array.Empty<object>());
        foreach (var property in userType.GetProperties())
        {
            var claimType = _optionsMonitor.CurrentValue.GetClaimType(property.Name);
            if (claimType == null)
                continue;

            string? claimValue = ClaimsPrincipal.FindClaimValue(claimType);
            object? claimTypeValue = null;
            try
            {
                claimTypeValue = TypeConvertProvider.ConvertTo(claimValue, property.PropertyType);
            }
            catch
            {
                claimTypeValue = this.ParseNonJson(property);
            }

            if (claimValue != null)
            {
                modelRelation.Setters[property]
                    .Invoke(userModel, new[] { claimTypeValue });
            }
        }

        return userModel;
    }

    private object? ParseNonJson(PropertyInfo property)
    {
        var claimValues = new List<string>();
        var claimType = _optionsMonitor.CurrentValue.GetClaimType(property.Name);
        if (claimType == null)
            return null;

        if (property.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
        {
            var claimsValues = ClaimsPrincipal?.Claims.Where(claim => claim.Type == claimType)
                .Select(claim => claim.Value).ToList();

            claimsValues?.ForEach(item =>
            {
                try
                {
                    var claimsValue = JsonSerializer.Deserialize<List<string>>(item);
                    if (claimsValue?.Any() == true)
                        claimValues.AddRange(claimsValue);
                }
                catch
                {
                    claimValues.Add(item);
                }
            });
        }

        return TypeConvertProvider.ConvertTo(JsonSerializer.Serialize(claimValues), property.PropertyType);
    }
}
