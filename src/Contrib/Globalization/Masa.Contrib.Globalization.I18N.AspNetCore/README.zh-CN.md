中 | [EN](README.md)

提供解析获取Culture的能力，配合[I18N](../Masa.Contrib.Globalization.I18N/README.zh-CN.md)来使用，目前支持三种方式进行切换语言:

* URL 参数 方式： ?culture=en-US，此方式优先级最高，格式为：culture=区域码
* Cookies 方式：调用 L.SetCulture(区域码) 方式切换
* 客户端浏览器语言自动匹配：如果前面两种方式都没有设置，支持自动根据客户端浏览器语言进行匹配。

## Masa.Contrib.Globalization.I18N.AspNetCore

用例：

``` powershell
Install-Package Masa.Contrib.Globalization.I18N.AspNetCore
```

### 入门

1. 默认资源文件夹结构

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
    "Home":"首页",
    "Docs":"文档",
    "Blog":"博客",
    "Team":"团队",
    "Search":"搜索",
    "User":{
        "Name":"名称"
    }
}
```

* supportedCultures.json

``` supportedCultures.json
[
    {
        "Culture":"zh-CN",
        "DisplayName":"中文简体",
        "Icon": "{Replace-Your-Icon}"
    },
    {
        "Culture":"en-US",
        "DisplayName":"English (United States)",
        "Icon": "{Replace-Your-Icon}"
    }
]
```

2. 注册使用I18N, 修改`Program.cs`

``` C#
services.AddI18N();
```

3. 使用`Masa.Contrib.Globalization.I18N.AspNetCore`提供解析Culture的能力

``` C#
app.UseI18N();
```

4. 如何使用I18N

* 从DI获取`II18N` (**II18N**是接口，支持从DI获取)
* 使用`I18N` (**I18N**是静态类)

以`I18N`为例:

``` C#
var home = I18N.T("Home"); //获取键值Home对应语言的值，此方法调用将返回"首页";
var name = I18N.T("User.Name");//输出：名称（支持嵌套）
```