中 | [EN](README.md)

## Masa.Contrib.RulesEngine.MicrosoftRulesEngine

用例：

``` powershell
Install-Package Masa.Contrib.RulesEngine.MicrosoftRulesEngine //使用 microsoft 提供的规则引擎
```

### 入门

1. 注册`RulesEngine`, 修改`Program.cs`

``` C#
builder.Services.AddRulesEngine(rulesEngineOptions =>
{
    rulesEngineOptions.UseRulesEngine();
})
```

2. 使用规则引擎Client ([`IRulesEngineClient`](../../../BuildingBlocks/RulesEngine/Masa.BuildingBlocks.RulesEngine/IRulesEngineClient.cs))

``` C#
IRulesEngineClient rulesEngineClient; //从DI获取

var json = @"{
  ""WorkflowName"": ""UserInputWorkflow"",
  ""Rules"": [
    {
      ""RuleName"": ""CheckAge"",
      ""ErrorMessage"": ""Must be over 18 years old."",
      ""ErrorType"": ""Error"",
      ""RuleExpressionType"": ""LambdaExpression"",
      ""Expression"": ""Age > 18""
    }
  ]
}";//规则json

var result = rulesEngineClient.Execute(ruleJson, new
{
    Age = 19
});
Console.WriteLine("规则执行结果为{0}", result);
```

### 进阶

### 扩展支持的方法

默认规则引擎不支持除`System`命名空间以外的方法，但你可以通过更改默认配置支持其它方法

1. 新建`StringUtils`类，用于扩展字符串方法，并为规则引擎中提供扩展方法

``` C#
public static class StringUtils
{
    public static bool IsNullOrEmpty(this string? value)
        => string.IsNullOrEmpty(value);
}
```

2. 注册`RulesEngine`, 并扩展支持`StringUtils`类提供的方法, 修改`Program.cs`

``` C#
builder.Services.AddRulesEngine(rulesEngineOptions =>
{
    rulesEngineOptions.UseRulesEngine(new ReSettings()
    {
        CustomTypes = new[] { typeof(StringUtils) }
    });
})
```

### 存在多个不同配置的规则引擎

支持多个完全不同的规则引擎，例如：

1. 注册`RulesEngine`, 并为当前规则引擎指定`name`

下面示例注册了两个规则引擎，默认规则引擎仅支持使用`System`命名空间的方法，name为`ruleA`的规则引擎除了支持`System`命名空间的方法之外还支持`StringUtils`类下的扩展方法

``` C#
builder.Services.AddRulesEngine(rulesEngineOptions =>
{
    rulesEngineOptions.UseRulesEngine();
    rulesEngineOptions.UseRulesEngine("ruleA", new ReSettings()
    {
        CustomTypes = new[] { typeof(StringUtils) }
    });
})
```

2. 使用指定的规则引擎

* 使用name为`ruleA`的规则引擎

``` C#
IRulesEngineFactory rulesEngineFactory; // 从DI获取
var rulesEngineClient = rulesEngineFactory.Create("ruleA");
```

* 使用默认的规则引擎

你可以通过DI直接获取到默认的规则引擎Client

``` C#
IRulesEngineClient rulesEngineClient; //从DI获取 (直接获取到的规则引擎Client是默认的)
```

或者通过规则引擎工厂获取

```　C#
IRulesEngineFactory rulesEngineFactory; // 从DI获取
rulesEngineClient = rulesEngineFactory.Create();
```