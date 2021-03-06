using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AgvcAgent.Api.Filters.GlobalFilters
{ 
    public class ApplicationErrorResult : ObjectResult
    {
        public ApplicationErrorResult(object value) : base(value)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
    /// <summary>
    ///     api异常统一处理过滤器
    ///     系统级别异常 500 应用级别异常501
    /// </summary>
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Result = BuildExceptionResult(context.Exception);
            base.OnException(context);
        }
       
        /// <summary>
        ///     包装处理异常格式
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private ObjectResult BuildExceptionResult(Exception ex)
        {
            var code = 0;
            var exception = "";
            //应用程序业务级异常
            if (ex is ApplicationException)
            {
                code = 501;
                exception = ex.Message;
            }
            else
            {
                // exception 系统级别异常，不直接明文显示的
                code = 500;
                exception = ex.Message;
            }

            if (ex.InnerException != null && ex.Message != ex.InnerException.Message)
                exception += "," + ex.InnerException.Message;

            
            return new ApplicationErrorResult(new ApiResult<string>()
            {
                Status = code,
                Error = exception,
                Success = false
            });
        }
    }
}