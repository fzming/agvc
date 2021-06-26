using System;

namespace AgvcAgent.Api.Filters.GlobalFilters
{
    /// <summary>
    ///     不记录监控接口执行日志
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NoLogAttribute : Attribute
    {
    }
}