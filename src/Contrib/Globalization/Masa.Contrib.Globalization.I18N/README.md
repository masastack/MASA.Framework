[中](README.zh-CN.md) | EN

Provide [Internationalization](https://developer.mozilla.org/zh-CN/docs/Mozilla/Add-ons/WebExtensions/Internationalization) capability

## Masa.Contrib.Globalization.I18N

Example:

``` powershell
Install-Package Masa.Contrib.Globalization.I18N
```

### getting Started

1. Default resource folder structure

``` structure
- Resources
  - I18N
    - en-US.json
    - zh-CN.json
    - supportedCultures.json
```

* en-US.json

``` en-US.json
{
    "Home":"Home",
    "Docs":"Docs",
    "Blog":"Blog",
    "Team":"Team",
    "Search":"Search"
    "User":{
        "Name":"Name"
    }
}
```

* zh-CN.json

``` zh-CN.json
{
    "Home":"Home",
    "Docs":"Documents",
    "Blog":"Blog",
    "Team":"Team",
    "Search":"Search",
    "User":{
        "Name":"Name"
    }
}
```

* supportedCultures.json

``` supportedCultures.json
[
    {
        "Culture":"zh-CN",
        "DisplayName":"Simplified Chinese",
        "Icon": "{Replace-Your-Icon}"
    },
    {
        "Culture":"en-US",
        "DisplayName":"English (United States)",
        "Icon": "{Replace-Your-Icon}"
    }
]
```

2. Register to use I18N, modify `Program.cs`

``` C#
services.AddI18n();
```

3. How to use I18N

* Get `II18n` from DI (**II18n** is the interface, supports getting from DI)
* Use `I18N` (**I18N** is a static class)

Take `I18N` as an example:

``` C#
var home = I18N.T("Home");//Get the value of the language corresponding to the key value Home, this method call will return "Home";
var name = I18N.T("User.Name");//Output: name (support nesting)
```

### Provided by I18N

* SetCulture (string cultureName): Switch CurrentCulture to zh-CN, and the format of numbers, dates, etc. will also change after it is changed
* SetUiCulture (string cultureName): Switch the interface language (CurrentUICulture) to zh-CN
* GetCultureInfo (): Get CurrentCulture configuration
* GetUiCultureInfo (): Get the interface language (CurrentUICulture)
* T (string name): get the value of the parameter 'name' in the current language
    * Returns the value of parameter 'name' when parameter 'name' is not configured in the current language
* T (string name, bool returnKey): get the value of the parameter 'name' in the current language
    * returnKey: whether to return the value of the parameter `name` when the parameter 'name' is not configured in the current language
* string T(string name, params object[] arguments): Get the value of the parameter 'name' in the current language, and get the final result according to the incoming parameters
    * arguments: parameter value, for example: '{0}' must be greater than 18, then I18N.T("Age",18) should be used
    * If the parameter has a format such as number, date, etc., it will change with the change of CurrentCulture
* string T(string name, bool returnKey, params object[] arguments): Get the value of the parameter 'name' in the current language, and get the final result according to the incoming parameters
* GetLanguages(): Get all languages supported by the current system