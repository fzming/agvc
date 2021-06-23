using System;
using System.Reflection;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Utility
{
    /// <summary>
    /// 基于Nito.AsyncEx.AsyncContext的异步转同步执行器
    /// </summary>
    public static class AsyncHelper
    {
        public static bool IsAsync(this MethodInfo method)
        {
            Check.NotNull(method, nameof(method));
            return method.ReturnType.IsTaskOrTaskOfT();
        }

        public static bool IsTaskOrTaskOfT(this Type type)
        {
            if (type == typeof(Task))
                return true;
            if (type.GetTypeInfo().IsGenericType)
                return type.GetGenericTypeDefinition() == typeof(Task<>);
            return false;
        }

        public static Type UnwrapTask(Type type)
        {
            Check.NotNull(type, nameof(type));
            if (type == typeof(Task))
                return typeof(void);
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
                return type.GenericTypeArguments[0];
            return type;
        }

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return AsyncContext.Run(func);
        }

        public static void RunSync(Func<Task> action)
        {
            AsyncContext.Run(action);
        }
    }
}