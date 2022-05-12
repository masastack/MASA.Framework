[中](README.zh-CN.md) | EN

## Masa.Contrib.Storage.ObjectStorage.Aliyun

Example：

````C#
Install-Package Masa.Contrib.Storage.ObjectStorage.Aliyun
````

support:
* GetSecurityToken to get the security token

### Usage 1:

1. Configure appsettings.json
```` C#
{
  "AliyunOss": {
    "AccessKeyId": "Replace-With-Your-AccessKeyId",
    "AccessKeySecret": "Replace-With-Your-AccessKeySecret",
    "RegionId": "Replace-With-Your-RegionId",
    "RoleArn": "Replace-With-Your-RoleArn",
    "RoleSessionName": "Replace-With-Your-RoleSessionName",
    "DurationSeconds": 3600,//optional, default: 3600s
    "Policy": "",//optional
    "TemporaryCredentialsCacheKey": "Aliyun.TemporaryCredentials"//optional, default: Aliyun.TemporaryCredentials
  }
}
````

2. Add Alibaba Cloud Storage Service

````C#
builder.Services.AddAliyunStorage();
````

### Usage 2:

1. Add Alibaba Cloud Storage Service

````C#
builder.Services.AddAliyunStorage(new AliyunStorageOptions("AccessKeyId", "AccessKeySecret", "regionId", "roleArn", "roleSessionName"));
````

### Usage 3:

1. Add Alibaba Cloud Storage Service

````C#
builder.Services.AddAliyunStorage(() => new AliyunStorageOptions(configuration["Aliyun:AccessKeyId"], configuration["Aliyun:AccessKeySecret"], configuration["Aliyun:RegionId"], configuration["Aliyun:RoleArn"], configuration ["Aliyun:RoleSessionName"]));
````

> The difference from usage 2 is that the configuration can take effect without restarting the project after the configuration update