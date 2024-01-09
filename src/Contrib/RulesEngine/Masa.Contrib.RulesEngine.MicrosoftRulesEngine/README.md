[中](README.zh-CN.md) | EN

## Masa.Contrib.RulesEngine.MicrosoftRulesEngine

A rules engine implemented based on [`RulesEngine`](https://github.com/microsoft/RulesEngine), It provides a simple way of giving you the ability to put your rules in a store outside the core logic of the system, thus ensuring that any change in rules don't affect the core system.

Example:

``` powershell
Install-Package Masa.Contrib.RulesEngine.MicrosoftRulesEngine //Use the rule engine provided by microsoft
```

### getting Started

1. Register `RulesEngine`, modify `Program.cs`

``` C#
builder.Services.AddRulesEngine(rulesEngineOptions =>
{
    rulesEngineOptions.UseMicrosoftRulesEngine();
})
```

2. Use Rule Engine Client ([`IRulesEngineClient`](../../../BuildingBlocks/RulesEngine/Masa.BuildingBlocks.RulesEngine/IRulesEngineClient.cs))

``` C#
IRulesEngineClient rulesEngineClient; //Get from DI

var json = @"{
  ""WorkflowName"": ""UserInputWorkflow"",// Not required
  ""Rules"": [
    {
      ""RuleName"": ""CheckAge"",
      ""ErrorMessage"": ""Must be over 18 years old."",
      ""ErrorType"": ""Error"",
      ""RuleExpressionType"": ""LambdaExpression"",
      ""Expression"": ""Age > 18""
    }
  ]
}";//rule json

var result = rulesEngineClient.Execute(json, new
{
    Age = 19
});
Console.WriteLine("The result of the rule execution is {0}", result[0].IsValid);
```

### Advanced

### Extended supported methods

The default rule engine does not support methods other than the `System` namespace, but you can support other methods by changing the default configuration

1. Create a new `StringUtils` class to extend string methods and provide extension methods for the rule engine

``` C#
public static class StringUtils
{
    public static bool IsNullOrEmpty(this string? value)
        => string.IsNullOrEmpty(value);
}
```

2. Register `RulesEngine`, and extend to support the methods provided by `StringUtils` class, modify `Program.cs`

``` C#
builder.Services.AddRulesEngine(rulesEngineOptions =>
{
    rulesEngineOptions.UseMicrosoftRulesEngine(new ReSettings()
    {
        CustomTypes = new[] { typeof(StringUtils) }
    });
})
```

### There are multiple rule engines with different configurations

Supports multiple completely different rule engines, such as:

1. Register `RulesEngine`, and specify `name` for the current rules engine

The following example registers two rule engines. The default rule engine only supports methods using the `System` namespace. The rule engine named `ruleA` supports the methods in the `StringUtils` class in addition to the methods in the `System` namespace. extension method

``` C#
builder.Services.AddRulesEngine(rulesEngineOptions =>
{
    rulesEngineOptions.UseMicrosoftRulesEngine();
    rulesEngineOptions.UseMicrosoftRulesEngine("ruleA", new ReSettings()
    {
        CustomTypes = new[] { typeof(StringUtils) }
    });
})
```

2. Use the specified rule engine

* Use the rule engine named `ruleA`

``` C#
IRulesEngineFactory rulesEngineFactory; // get from DI
var rulesEngineClient = rulesEngineFactory.Create("ruleA");
```

* Use the default rule engine

You can directly get the default rule engine Client through DI

``` C#
IRulesEngineClient rulesEngineClient; //Get from DI (the directly obtained rule engine Client is the default)
```

Or get it through the rule engine factory

``` C#
IRulesEngineFactory rulesEngineFactory; // get from DI
rulesEngineClient = rulesEngineFactory.Create();
```
