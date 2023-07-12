// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Caller;

public abstract class StackHttpClientCaller : HttpClientCallerBase
{
    protected override void UseHttpClientPost(MasaHttpClientBuilder masaHttpClientBuilder)
    {
        masaHttpClientBuilder.UseAuthentication(serviceProvider => new AuthenticationService(
                serviceProvider.GetRequiredService<TokenProvider>(),
                serviceProvider.GetRequiredService<JwtTokenValidator>(),
                serviceProvider.GetRequiredService<IMultiEnvironmentContext>()
            ));
        base.UseHttpClientPost(masaHttpClientBuilder);
    }
}
