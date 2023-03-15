// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Caller;

public class StackHttpClientCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; }

    public StackHttpClientCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }


    protected override void UseHttpClientPost(MasaHttpClientBuilder masaHttpClientBuilder)
    {
        masaHttpClientBuilder.UseAuthentication(serviceProvider => new AuthenticationService(
                serviceProvider.GetRequiredService<TokenProvider>(),
                serviceProvider.GetRequiredService<JwtTokenValidator>()
            ));
        base.UseHttpClientPost(masaHttpClientBuilder);
    }
}
