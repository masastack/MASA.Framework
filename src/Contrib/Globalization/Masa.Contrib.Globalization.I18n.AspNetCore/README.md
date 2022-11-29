[中](README.zh-CN.md) | EN

Provides the ability to parse and obtain Culture, and use it with [I18n](../Masa.Contrib.Globalization.I18n/README.md). Currently, there are three ways to switch languages:

* URL parameter method: ?culture=en-US, this method has the highest priority, the format is: culture=region code
* Cookies method: the cookie format is c=%LANGCODE%|uic=%LANGCODE%, where c is Culture and uic is UICulture, for example:

``` cookie
c=en-UK|uic=en-US
```

* Client browser language automatic matching: If the previous two methods are not set, it supports automatic matching according to the client browser language.

> Detailed [reference](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-7.0#localization-middleware)

## Masa.Contrib.Globalization.I18n.AspNetCore

Example:

``` powershell
Install-Package Masa.Contrib.Globalization.I18n.AspNetCore
```

### getting Started

1. Default resource folder structure

``` structure
- Resources
  - I18n
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

2. Register to use I18n, modify `Program.cs`

``` C#
services.AddI18n();
```

3. Use `Masa.Contrib.Globalization.I18n.AspNetCore` to provide the ability to parse Culture

``` C#
app.UseI18n();
```

4. How to use I18n

* Get `II18n` from DI (**II18n** is the interface, supports getting from DI)
* Use `I18n` (**I18n** is a static class)

Take `I18n` as an example:

```` C#
var home = I18n.T("Home"); //Get the value of the language corresponding to the key value Home, this method call will return "Home";
var name = I18n.T("User.Name");//Output: name (support nesting)
````