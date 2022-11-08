// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18N.BlazorWebAssembly;

[ExcludeFromCodeCoverage]
public class I18NOfT<TResourceSource> : BuildingBlocks.Globalization.I18N.I18NOfT<TResourceSource>
{
    private const string CULTURE_COOKIE_KEY = ".AspNetCore.Culture";

    private const string UI_CULTURE_COOKIE_KEY = ".AspNetCore.UICulture";

    private const string GET_COOKIE_JS = @"(function(name){
       return window.localStorage[name];
    })";

    private const string SET_COOKIE_JS = @"(function(name, value){
       return window.localStorage[name] = value;
    })";

    private readonly IJSInProcessRuntime _jsRuntime;
    private readonly IOptions<CultureSettings> _options;

    public I18NOfT(IJSRuntime jsRuntime, IOptions<CultureSettings> options)
    {
        _jsRuntime = (IJSInProcessRuntime)jsRuntime;
        _options = options;
    }

    public override CultureInfo GetCultureInfo() => GetSelectCulture(CULTURE_COOKIE_KEY);

    public override CultureInfo GetUiCultureInfo() => GetSelectCulture(UI_CULTURE_COOKIE_KEY);

    public override void SetCulture(CultureInfo culture)
        => SetCultureCore(CULTURE_COOKIE_KEY, culture);

    public override void SetUiCulture(CultureInfo culture) => SetCultureCore(UI_CULTURE_COOKIE_KEY, culture);

    private CultureInfo GetSelectCulture(string name)
    {
        var result = _jsRuntime.Invoke<string?>("eval", $"{GET_COOKIE_JS}('{name}')");
        return new(result ?? _options.Value.DefaultCulture);
    }

    private void SetCultureCore(string name, CultureInfo culture)
    {
        try
        {
            _jsRuntime.InvokeVoidAsync("eval", $"{SET_COOKIE_JS}('{name}','{culture.Name}')");
        }
        catch (Exception ex)
        {
        }
    }
}
