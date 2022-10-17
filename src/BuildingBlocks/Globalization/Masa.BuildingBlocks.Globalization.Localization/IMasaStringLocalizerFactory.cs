// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.Localization;

public interface IMasaStringLocalizerFactory
{
    IMasaStringLocalizer Create<TResourceSource>();

    IMasaStringLocalizer Create(Type resourceSource);
}
