using System;

namespace AgvcAgent.Api.Filters.GlobalFilters
{
    /// <summary>
    ///     标识Action不采用统一结果模型进行数据返回
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class IgnoreResultModelAttribute : Attribute
    {
    }
}