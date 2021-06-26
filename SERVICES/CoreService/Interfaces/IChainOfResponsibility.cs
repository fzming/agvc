namespace CoreService.Interfaces
{
    /// <summary>
    ///     责任链模式接口
    /// </summary>
    public interface IChainOfResponsibility<T> where T : class
    {
        /// <summary>
        ///     优先级
        /// </summary>
        int Priority { get; }

        /// <summary>
        ///     是否启用
        /// </summary>
        bool Enable { get; }

        /// <summary>
        ///     下一个服务
        /// </summary>
        T Next { get; set; }
    }
}