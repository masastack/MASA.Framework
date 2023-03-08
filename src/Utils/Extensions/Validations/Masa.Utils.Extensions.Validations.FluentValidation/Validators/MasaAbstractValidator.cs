// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

using System;
using System.Linq.Expressions;
using static System.Collections.Specialized.BitVector32;

namespace FluentValidation.Validators;

public abstract class MasaAbstractValidator<T> : AbstractValidator<T>
{
    protected virtual IConditionBuilder WhenNotEmpty<TProperty>(Expression<Func<T, TProperty?>> selector, Action<IRuleBuilderInitial<T, TProperty?>> action) where TProperty : class
    {
        return WhenNotEmpty(selector, selector, action);
    }
    protected virtual IConditionBuilder WhenNotEmpty<TProperty, TValidProperty>(Expression<Func<T, TProperty?>> selector, Expression<Func<T, TValidProperty?>> validSelector, Action<IRuleBuilderInitial<T, TValidProperty?>> action) where TProperty : class
    {
        return When(GetPredicate(selector), () => action.Invoke(RuleFor(validSelector)));
    }
    protected virtual IConditionBuilder WhenNotEmpty<TProperty>(Expression<Func<T, TProperty?>> selector, IPropertyValidator<T, TProperty?> validator) where TProperty : class
    {
        return WhenNotEmpty(selector, selector, validator);
    }

    protected virtual IConditionBuilder WhenNotEmpty<TProperty, TValidProperty>(Expression<Func<T, TProperty?>> selector, Expression<Func<T, TValidProperty?>> validSelector, IPropertyValidator<T, TValidProperty?> validator) where TProperty : class
    {
        return When(GetPredicate(selector), () => RuleFor(validSelector).SetValidator(validator));
    }

    protected virtual Func<T, bool> GetPredicate<TProperty>(Expression<Func<T, TProperty?>> selector)
    {
        var selectorFunc = selector.Compile();
        return value =>
        {
            var propertyValue = selectorFunc(value);
            switch (propertyValue)
            {
                case null:
                case string s when string.IsNullOrWhiteSpace(s):
                case System.Collections.ICollection { Count: 0 }:
                case Array { Length: 0 }:
                case System.Collections.IEnumerable e when !e.GetEnumerator().MoveNext():
                    return false;
            }
            return !EqualityComparer<TProperty>.Default.Equals(propertyValue, default);
        };
    }
}
