// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Internal;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal static class MasaAppConfig
{
    public static string AppId() => MasaApp.GetService<IOptions<MasaAppConfigureOptions>>()?.Value.AppId ?? string.Empty;
}
