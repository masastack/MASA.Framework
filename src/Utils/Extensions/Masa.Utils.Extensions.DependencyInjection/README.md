[中](README.zh-CN.md) | EN

## Masa.Utils.Extensions.DependencyInjection

Example：

``` powershell
Install-Package Masa.Utils.Extensions.DependencyInjection
```

### Get Started

````C#
services.AddAutoInject();
````

### Advanced

#### Dependent interface:

* ISingletonDependency: registers a service whose lifecycle is Singleton
* IScopedDependency: registers a service whose lifecycle is Scoped
* ITransientDependency: registers services whose lifecycle is Transient
* IAutoFireDependency: is automatically triggered (used in combination with ISingletonDependency, IScopedDependency, and ITransientDependency to trigger a service acquisition operation after the service is automatically registered, only inheriting IAutoFireDependency does not work)

Example:

````c#
public interface IRepository<TEntity> : IScopedDependency
    where TEntity : class
{

}
````

> Because IRepository<TEntity> inherits IScopedDependency, the life cycle of IRepository<TEntity> will be Scoped

#### Rule:

Scan the interfaces and classes that inherit ISingletonDependency, IScopedDependency, and ITransientDependency in the assembly, and automatically register services for them

* When inheriting an interface, its ServiceType is the current interface, and its ImplementationType is the implementation class of the current interface
   * If the current interface has multiple implementation classes, it will be added multiple times

     ```` C#
     public interface IUserService : IScopedDependency
     {

     }

     public class UserService : IUserService
     {

     }
     ````
     > Equivalent to service.AddScoped<IUserService, UserService>();

  * If you want the interface to have only one implementation class, add [Dependency(ReplaceServices = true)] above the implementation class

      ```` C#
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
      ````
  > Equivalent to service.AddScoped<IUserService, UserService2>();

* When the inherited class is not an interface, its ServiceType is the current class, and its ImplementationType is also the current class
   * By default, the cascade scan registration service is supported, and subclasses of the current class will also be registered

     ```` C#
     public class RepositoryBase : ISingletonDependency
     {

     }

     /// <summary>
     /// Abstract classes are not automatically registered
     /// </summary>
     public abstract class CustomizeBaseRepository : ISingletonDependency
     {

     }

     public class UserRepository : RepositoryBase
     {

     }
     ````

     > Equivalent to: `service.AddSingleton<RepositoryBase>();service.AddSingleton<UserRepository>();`

#### Features:

* IgnoreInjection: Ignore injection, used to exclude not being injected automatically
* Dependency:
    * TryRegister: Set true to be registered only when the service is not registered, similar to TryAdd of IServiceCollection... extension method
    * ReplaceServices: Set true to replace previously registered services, similar to the Replace... extension method of IServiceCollection.

#### Methods:

* Extend IServiceCollection
* GetInstance<TService>(): Get the instance of service T
    * Any<TService>(): Whether there is a service TService, does not support generic services
    * Any<TService, TImplementation>(): Whether there is a service whose interface is TService and whose implementation class is TImplementation
    * Any<TService>(ServiceLifetime.Singleton): Whether there is a service TService with a life cycle of Singleton (generic services are not supported)
    * Any<TService, TImplementation>(ServiceLifetime.Singleton): Is there an interface whose life cycle is Singleton as TService and is implemented as a TImplementation service (generic services are not supported)
    * Replace<TService>(typeof(TImplementation), ServiceLifetime.Singleton): Remove the first service with the same service type in the service collection, and add typeof(TImplementation) to the collection, the life cycle is a singleton
    * ReplaceAll<TService>(typeof(TImplementation), ServiceLifetime.Singleton): Remove all services with the same service type in the service collection, and add typeof(TImplementation) to the collection, the life cycle is a singleton