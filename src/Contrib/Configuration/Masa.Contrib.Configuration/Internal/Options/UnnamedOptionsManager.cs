// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

/// <summary>
/// Provider for default IOptions
/// </summary>
/// <typeparam name="TOptions"></typeparam>
internal sealed class UnnamedOptionsManager<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]TOptions> :
    IOptions<TOptions>
    where TOptions : class
{
    private readonly Lazy<TOptions> _valueLazy;

    public UnnamedOptionsManager(IOptionsFactory<TOptions> factory)
        => _valueLazy = new Lazy<TOptions>(() => factory.Create(Options.DefaultName), true);

    public TOptions Value => _valueLazy.Value;
}
