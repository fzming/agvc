namespace Utility
{
    /// <summary>
    /// 实现接口的类将自动注册到Ioc容器
    /// </summary>
    public interface IDependency
    {
    }
    /// <summary>
    /// 实现该接口将自动注册到Ioc容器，生命周期为每次请求创建一个实例
    /// </summary>
    public interface IScopeDependency : IDependency
    {
    }
    /// <summary>
    /// 实现该接口将自动注册到Ioc容器，生命周期为单例
    /// </summary>
    public interface ISingletonDependency : IDependency
    {
    }
    /// <summary>
    /// 实现该接口将自动注册到Ioc容器，生命周期为每次创建一个新实例
    /// </summary>
    public interface ITransientDependency : IDependency
    {
    }
}