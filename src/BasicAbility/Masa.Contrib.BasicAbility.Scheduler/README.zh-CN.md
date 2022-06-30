中 | [EN](README.md)

## Masa.Contrib.BasicAbility.Scheduler

作用：

通过Scheduler Client 操作Scheduler服务的API

```c#
ISchedulerClient
├── SchedulerJobService                 Job服务
├── SchedulerTaskService                Task服务
```

用例：

```C#
Install-Package Masa.Contrib.BasicAbility.Scheduler
```

```C#
builder.Services.AddSchedulerClient("Scheduler service address");
```

如何使用：

```c#
var app = builder.Build();

app.MapGet("/startjob", ([FromServices] ISchedulerClient schedulerClient) =>
{
    var request = new BaseSchedulerJobRequest()
    {
        JobId = new Guid("39351BF4-0E58-463F-5C96-08DA42DF67D6"),
        OperatorId = new Guid("15905535-C90F-4BCA-467B-08DA42E0A2C0")
    };
    return schedulerClient.SchedulerJobService.StartAsync(request);
});

app.Run();
```
