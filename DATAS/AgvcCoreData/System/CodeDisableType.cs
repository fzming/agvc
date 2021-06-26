namespace AgvcCoreData.System
{
    /// <summary>
    ///     无指令权限时，按钮表现类型
    /// </summary>
    public enum CodeDisableType
    {
        /// <summary>
        ///     按钮将在页面中隐藏
        /// </summary>
        隐藏,

        /// <summary>
        ///     按钮将表现为禁用状态
        /// </summary>
        禁用,

        /// <summary>
        ///     按钮正常显示，但是在点击时判断没有权限，将提示无权限
        /// </summary>
        提示
    }
}