using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AgvcAgent.Api.Filters.GlobalFilters
{
    /// <summary>
    /// api异常统一处理过滤器
    /// 系统级别异常 500 应用级别异常501
    /// </summary>
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.Result = BuildExceptionResult(context.Exception);
            base.OnException(context);
        }

        /// <summary>
        /// 包装处理异常格式
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private ObjectResult BuildExceptionResult(Exception ex)
        {
            int code = 0;
            string msg = "";
            string exception = "";
            //应用程序业务级异常
            if (ex is ApplicationException)
            {
                code = 501;
                msg = ex.Message;
            }
            else
            {
                // exception 系统级别异常，不直接明文显示的
                code = 500;
                msg = "发生系统级别异常";
                exception = ex.Message;
            }

            if (ex.InnerException != null && ex.Message != ex.InnerException.Message)
                exception += "," + ex.InnerException.Message;

            return new ObjectResult(new { code, msg, innerMessage = exception });
        }
    }
}