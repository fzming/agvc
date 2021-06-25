using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AgvcAgent.Api.Filters.GlobalFilters
{
    /// <summary>
    /// 在Action运行的前、后运行执行过滤器
    /// </summary>
    public class ApiActionFilter : ActionFilterAttribute
    {


        public override void OnResultExecuting(ResultExecutingContext context)
        {
          
            if (context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<IgnoreResultModelAttribute>() != null)
            {
                return;
            }

            //根据实际需求进行具体实现
            if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.Value == null)
                {
                    context.Result = new ObjectResult(new { code = 404, msg = "未找到资源" });
                }
                else
                {
                    context.Result = new ObjectResult(new { code = 200, msg = "ok", result = objectResult.Value });
                }
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(new { code = 404, msg = "" });
            }
            else if (context.Result is ContentResult result)
            {
                context.Result = new ObjectResult(new { code = 200, msg = "ok", result = result.Content });
            }
            else if (context.Result is StatusCodeResult codeResult)
            {
                context.Result = new ObjectResult(new
                { code = codeResult.StatusCode, msg = codeResult.StatusCode == 200 ? "ok" : "" });
            }
        }

    }


}