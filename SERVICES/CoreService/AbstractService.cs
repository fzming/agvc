using Nito.AsyncEx;

namespace CoreService
{
    /// <summary>
    ///     抽象服务基类
    /// </summary>
    public abstract class AbstractService
    {
        public readonly AsyncLock _mutex = new();
    }
}