[中](README.zh-CN.md) | EN

Provides the ability to parse and obtain Culture, and use it with [I18N](../Masa.Contrib.Globalization.I18n/README.md). Currently, there are three ways to switch languages:

* URL parameter method: ?culture=en-US, this method has the highest priority, the format is: culture=region code
* Cookies method: call L.SetCulture (region code) method to switch
* Client browser language automatic matching: If neither of the previous two methods are set, it supports automatic matching according to the client browser language.

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

2. Register to use I18N, modify `Program.cs`

``` C#
services.AddI18N();
```

3. Use `Masa.Contrib.Globalization.I18n.AspNetCore` to provide the ability to parse Culture

``` C#
app.UseI18N();
```

4. How to use I18N

* Get `II18N` from DI (**II18N** is the interface, supports getting from DI)
* Use `I18N` (**I18N** is a static class)

Take `I18N` as an example:

```` C#
var home = I18N.T("Home"); //Get the value of the language corresponding to the key value Home, this method call will return "Home";
var name = I18N.T("User.Name");//Output: name (support nesting)
````