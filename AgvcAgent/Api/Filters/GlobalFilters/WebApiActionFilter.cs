using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CoreData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AgvcAgent.Api.Filters.GlobalFilters
{
    /// <summary>
    /// 在Action运行的前、后运行执行过滤器
    /// </summary>
    public class WebApiActionFilter : ActionFilterAttribute
    {
        private const string TrackKey = "__action_duration__";

        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (!SkipLogging(actionContext))//是否该类标记为NoLog
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                //记录进入请求的时间
                actionContext.Request.Properties[TrackKey] = stopWatch;

            }
           
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        /// <summary>
        /// 通过重写 OnActionExecutingAsync，来拦截action的请求消息，
        /// 当执行OnActionExecutingAsync完成以后才真正进入请求的action中，action运行完后又把控制权给了 OnActionExecutedAsync ，
        /// 这个管道机制可以使我们用它来轻松实现 权限认证、日志记录 ，跨域以及很多需要对全局或者部分请求做手脚的的功能。
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            #region 有效性检查

            if (!(actionExecutedContext.Request.Properties[TrackKey] is Stopwatch stopWatch))
            {
                return;
            }
            stopWatch.Stop();
            var actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            var controllerName = actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
#if  DEBUG
            Trace.WriteLine($"[Execute {controllerName}- {actionName} took {stopWatch.ElapsedMilliseconds}.]");
#endif
            if (actionExecutedContext.Exception != null) //有错误跳出
            {
                return;
            }
            var context = actionExecutedContext.ActionContext;
            var response = context.Response;
            
            var ignoreResultModelAttributes = context.ActionDescriptor.GetCustomAttributes<IgnoreResultModelAttribute>();
            if (ignoreResultModelAttributes.Any())
            {
                await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
                return;
            }

            #endregion

            var returnType = context.ActionDescriptor.ReturnType;
            //导出文件Excel
            if (returnType == typeof(AttachmentFile))
            {
                var attach = await response.Content.ReadAsAsync<AttachmentFile>(cancellationToken);
                var r = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new MemoryStream(attach.Content);
                r.Content = new StreamContent(stream);
                r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                r.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = FixAttachmentFileName(attach.FileName)
                };
                //暴露Access-Control-Expose-Headers
                r.Content.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                actionExecutedContext.Response = r;
                return;
            }
            //封装ApiResult
            var result = new ApiResult<dynamic>
            {
                Status = response.StatusCode,
                Success = response.IsSuccessStatusCode
            };
            if (response.TryGetContentValue(out dynamic content))
            {
                if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Result<>))
                {
                    result.Success = content.Success;
                    result.Error = content.Error;
                    result.Data = content.Data;
                    result.Status = result.Success ? result.Status : HttpStatusCode.InternalServerError;

                }
                else
                {
                    result.Data = content;
                }

            }

            //响应处理
            var newResponse = actionExecutedContext.Request.CreateResponse(result.Status, result);
            newResponse.Headers.Add("ExecutionTime",
                stopWatch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture));
            
            actionExecutedContext.Response = newResponse;

        }

        private string FixAttachmentFileName(string attachFileName)
        {
            try
            {
                var browser = string.Empty;
                if (HttpContext.Current.Request.UserAgent != null)
                {
                    browser = HttpContext.Current.Request.UserAgent.ToUpper();
                }

                return browser.Contains("FIREFOX")
                    ? attachFileName
                    : HttpUtility.UrlEncode(attachFileName);

            }
            catch
            {
                return HttpUtility.UrlEncode(attachFileName);
            }

        }
        private static bool SkipLogging(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<NoLogAttribute>().Any() || actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<NoLogAttribute>().Any();
        }
    }


}