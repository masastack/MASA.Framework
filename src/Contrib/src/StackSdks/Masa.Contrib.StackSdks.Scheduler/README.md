[中](README.zh-CN.md) | EN

## Masa.Contrib.StackSdks.Scheduler

Effect:

Operate API of scheduler service through scheduler client

```c#
ISchedulerClient
├── SchedulerJobService                 Scheduler Job service
├── SchedulerTaskService                Scheduler Task service
```

Example：

```C#
Install-Package Masa.Contrib.StackSdks.Scheduler
```

```C#
builder.Services.AddSchedulerClient("Scheduler service address");
```

How to use：

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
