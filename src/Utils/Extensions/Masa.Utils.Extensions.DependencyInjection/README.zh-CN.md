中 | [EN](README.md)

## Masa.Utils.Extensions.DependencyInjection

用例：

``` powershell
Install-Package Masa.Utils.Extensions.DependencyInjection
```

### 入门

```C#
services.AddAutoInject();
```

### 进阶

#### 依赖接口:

* ISingletonDependency: 注册生命周期为Singleton的服务
* IScopedDependency: 注册生命周期为Scoped的服务
* ITransientDependency: 注册生命周期为Transient的服务
* IAutoFireDependency: 自动触发（与ISingletonDependency、IScopedDependency、ITransientDependency结合使用，在服务自动注册结束后触发一次获取服务操作，仅继承IAutoFireDependency不起作用）

示例:

```c#
public interface IRepository<TEntity> : IScopedDependency
    where TEntity : class
{

}
```

> 因IRepository<TEntity>继承IScopedDependency，所以会将IRepository<TEntity>的生命周期为Scoped

#### 规则:

扫描程序集中继承ISingletonDependency、IScopedDependency、ITransientDependency的接口以及类，并为其自动注册服务

* 当继承的是接口时，其ServiceType是当前接口，其ImplementationType是当前接口的实现类
  * 如果当前接口有多个实现类，会被多次添加

    ``` C#
    public interface IUserService : IScopedDependency
    {

    }

    public class UserService : IUserService
    {

    }
    ```
    > 等价于 service.AddScoped<IUserService, UserService>();

  * 如果希望接口只有一个实现类，则在实现类上方增加[Dependency(ReplaceServices = true)]即可

    ``` C#
    public interface IUserService : IScopedDependency
    {

    }

    public class UserService : IUserService
    {

    }

    [Dependency(ReplaceServices = true)]
    public class UserService2 : IUserService
    {

    }
    ```
    > 等价于 service.AddScoped<IUserService, UserService2>();

* 当继承的类不是接口时，其ServiceType是当前类，其ImplementationType也是当前类
  * 默认支持级联扫描注册服务，当前类的子类也会被注册

    ``` C#
    public class BaseRepository : ISingletonDependency
    {

    }

    /// <summary>
    /// 抽象类不会被自动注册
    /// </summary>
    public abstract class CustomizeBaseRepository : ISingletonDependency
    {

    }

    public class UserRepository : BaseRepository
    {

    }
    ```

    > 等价于: `service.AddSingleton<BaseRepository>();service.AddSingleton<UserRepository>();`

#### 特性:

* IgnoreInjection: 忽略注入，用于排除不被自动注入
* Dependency:
  * TryRegister: 设置true则仅当服务未注册时才会被注册，类似IServiceCollection的TryAdd ... 扩展方法
  * ReplaceServices: 设置true则替换之前已经注册过的服务，类似IServiceCollection的Replace ... 扩展方法.

#### 方法:

* 扩展IServiceCollection
  * GetInstance<TService>(): 获取服务T的实例
  * Any<TService>(): 是否存在服务TService，不支持泛型服务
  * Any<TService, TImplementation>(): 是否存在接口为TService、且实现类为TImplementation的服务
  * Any<TService>(ServiceLifetime.Singleton): 是否存在一个生命周期为Singleton的服务TService(不支持泛型服务)
  * Any<TService, TImplementation>(ServiceLifetime.Singleton): 是否存在一个生命周期为Singleton的接口为TService，实现为TImplementation的服务(不支持泛型服务)
  * Replace<TService>(typeof(TImplementation), ServiceLifetime.Singleton): 移除服务集合中具有相同服务类型的第一个服务，并将 typeof(TImplementation) 添加到集合中，生命周期为单例
  * ReplaceAll<TService>(typeof(TImplementation), ServiceLifetime.Singleton): 移除服务集合中具有相同服务类型的所有服务，并将 typeof(TImplementation) 添加到集合中，生命周期为单例