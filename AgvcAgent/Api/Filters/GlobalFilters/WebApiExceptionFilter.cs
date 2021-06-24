using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AgvcAgent.Api.Filters.GlobalFilters
{
    /// <summary>
    /// API异常过滤器
    /// 当异常发生的时候运行
    /// </summary>
    public class WebApiExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// 当Controller执行异常发生时
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnException(ExceptionContext actionExecutedContext)
        {
            ProcessException(actionExecutedContext);
        }

        private void ProcessException(ExceptionContext actionExecutedContext)
        {
            // 取得发生异常时的错误讯息
            var errorMessage = actionExecutedContext.Exception.Message;

            var result = new ApiResult<object>
            {
                Status = GetStatusCode(actionExecutedContext.Exception),
                Error = errorMessage
            };
            //todo:记录错误信息

          
            // 重新打包回传的讯息
            actionExecutedContext.Response = actionExecutedContext.Request
                .CreateResponse(result.Status, result);
        }

        private HttpStatusCode GetStatusCode(Exception exception)
        {

            switch (exception)
            {
                case NotImplementedException _:
                    return HttpStatusCode.NotImplemented;
                case FileNotFoundException _:
                    return HttpStatusCode.NotFound;
                case ArgumentException _:
                    return HttpStatusCode.BadRequest;
                default:
                    return HttpStatusCode.InternalServerError;
            }
        }

        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext,
            CancellationToken cancellationToken)
        {
            var task = base.OnExceptionAsync(actionExecutedContext, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                return task;
            }
            return task.ContinueWith(t =>
            {
                ProcessException(actionExecutedContext);
            }, cancellationToken);
        }

    }
}