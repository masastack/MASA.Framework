[中](README.zh-CN.md) | EN

## Masa.Contrib.Storage.ObjectStorage.Aliyun

Example：

````C#
Install-Package Masa.Contrib.Storage.ObjectStorage.Aliyun
````

support:
* GetSecurityToken: Gets the security token(Sts RegionId and RoleArn and RoleSessionName are required)
* GetObjectAsync: Gets the stream of object data
* PutObjectAsync: Upload objects via Stream
* ObjectExistsAsync: Determine whether the object exists
* DeleteObjectAsync: Delete object

### Usage 1:

1. Configure appsettings.json
```` C#
{
  "Aliyun": {
    "AccessKeyId": "Replace-With-Your-AccessKeyId",
    "AccessKeySecret": "Replace-With-Your-AccessKeySecret",
    "Sts": :{
      "RegionId": "Replace-With-Your-RegionId",//https://www.alibabacloud.com/help/en/resource-access-management/latest/endpoints#reference-sdg-3pv-xdb
      "DurationSeconds": 3600,//Temporary certificate validity period, default: 3600s
      "EarlyExpires": 10//default: 10s
    },
    "Storage": {
      "Endpoint": "Replace-With-Your-Endpoint",//https://www.alibabacloud.com/help/en/object-storage-service/latest/regions-and-endpoints#section-plb-2vy-5db
      "RoleArn": "Replace-With-Your-RoleArn",
      "RoleSessionName": "Replace-With-Your-RoleSessionName",
      "TemporaryCredentialsCacheKey": "Aliyun.Storage.TemporaryCredentials",//optional, default: Aliyun.Storage.TemporaryCredentials
      "Policy": ""//optional
    }
  }
}
````

2. Add Aliyun Storage Service

````C#
builder.Services.AddAliyunStorage();
````

3. Get `IClient` from DI

     ```` C#
     //upload files
     var fileStream = File.OpenRead("D://favicon.png");//Replace the local file path
     await serviceProvider.GetService<IClient>().PutObjectAsync("storage1-test", "1.png", fileStream);
     ````

### Usage 2:

1. Add Aliyun Storage Service

````C#
var configuration = builder.Configuration;
var aliyunStorageOptions = new AliyunStorageOptions(
    configuration["Aliyun:AccessKeyId"],
    configuration["Aliyun:AccessKeySecret"],
    configuration["Aliyun:Endpoint"],
    configuration["Aliyun:RoleArn"],
    configuration["Aliyun:RoleSessionName"])
{
    Sts = new AliyunStsOptions(configuration["Aliyun:RegionId"]);
};
builder.Services.AddAliyunStorage(aliyunStorageOptions);
````

2. Get `IClient` from DI and use the corresponding method

### Usage 3:

1. Add Aliyun Storage Service

````C#
var configuration = builder.Configuration;
builder.Services.AddAliyunStorage(() =>
{
    return new AliyunStorageOptions(
        configuration["Aliyun:AccessKeyId"],
        configuration["Aliyun:AccessKeySecret"],
        configuration["Aliyun:Endpoint"],
        configuration["Aliyun:RoleArn"],
        configuration["Aliyun:RoleSessionName"])
    {
        Sts = new AliyunStsOptions(configuration["Aliyun:RegionId"])
    };
});
````

2. Get `IClient` from DI and use the corresponding method

> The difference from usage 2 is to defer getting the configuration

### Usage 4:

1. Add Aliyun Storage Service

````C#
builder.Services.AddAliyunStorage((serviceProvider) =>
{
     var configuration = serviceProvider.GetRequiredService<IConfiguration>();
     return new AliyunStorageOptions(
         configuration["Aliyun:AccessKeyId"],
         configuration["Aliyun:AccessKeySecret"],
         configuration["Aliyun:Endpoint"],
         configuration["Aliyun:RoleArn"],
         configuration["Aliyun:RoleSessionName"])
     {
         Sts = new AliyunStsOptions(configuration["Aliyun:RegionId"])
     };
});
````

2. Get `IClient` from DI and use the corresponding method

> The difference from usage 3 is that the service required for configuration can be obtained through serviceProvider, and finally the configuration object is returned

> If you do not need to use temporary credentials, you can not configure the parameters of Sts and RoleArn and RoleSessionName.