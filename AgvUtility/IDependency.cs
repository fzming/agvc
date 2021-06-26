namespace Utility
{
    /// <summary>
    ///实现接口的类将自动注册到Ioc容器
    /// </summary>
    public interface IDependency
    {
    }

    /// <summary>
    /// 实现该接口将自动注册到Ioc容器，生命周期为每次请求创建一个实例
    /// 范围生命周期被创建，一旦每个客户端请求时(connection)
    ///警告：当在中间件中使用范围服务时，注入服务到Invoke或者InvokeAsync方法。不要通过构造函数注入，因为那回强制服务表现的像是singleton(单例)
    /// </summary>
    public interface IScopeDependency : IDependency
    {
    }

    /// <summary>
    /// 实现该接口将自动注册到Ioc容器，生命周期为单例
    /// 单独生命周期在第一次请求时被创建(或者说当ConfigureService运行并且被service registration指定时)。之后每一个请求都使用同一个实例。如果应用要求一个单独行为(singleton behavior)，允许service container来管理服务生命周期是被推荐的。不要实现一个单例设计模式并且在类中提供用户代码来管理这个对象的生命周期。
    ///警告：从一个singleton来解析一个范围服务(scoped service)是危险的。它可能会造成服务有不正确的状态，当处理随后的请求时。
    /// </summary>
    public interface ISingletonDependency : IDependency
    {
    }

    /// <summary>
    /// 实现该接口将自动注册到Ioc容器，生命周期为每次创建一个新实例
    /// 临时的生命周期服务是在每次从服务容器中被请求时被创建。这个生命周期对于lightweight(轻量的),stateless(无状态的)服务比较合适。
    /// </summary>
    public interface ITransientDependency : IDependency
    {
    }
}