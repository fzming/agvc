using System.Net;
using CoreData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Utility.Extensions;

namespace AgvcAgent.Api.Filters.GlobalFilters
{
      /// <summary>
    /// 执行返回模型
    /// </summary>
    public class ApiResult<T>: Result<T>
    {
        [JsonProperty("status")]
        public int Status { get; set; }
    }
    /// <summary>
    ///     在Action运行的前、后运行执行过滤器
    /// </summary>
    public class ApiActionFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<SkipActionFilterAttribute>() != null) return;
            var response = context.HttpContext.Response;
            var result = new ApiResult<dynamic>
            {
                Status = response.StatusCode,
                Success = response.StatusCode==200
            };
            
            var returnType = context.Result.GetType();
            //根据实际需求进行具体实现
            if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.Value == null)
                    result.Success = false;
                else
                { 
                    if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Result<>))
                    {
                        objectResult.MapTo(result);
                        result.Status = result.Success ? result.Status : 500;

                    }
                    result.Data = objectResult.Value;
                }
            }
            else if (context.Result is EmptyResult)
            {
                result.Data = "";
            }
            else if (context.Result is ContentResult r)
            {
                result.Data = r.Content;
            }
            else if (context.Result is StatusCodeResult codeResult)
            {
                result.Data= codeResult.StatusCode == 200 ? "ok" : "";
            }
            context.Result = new ObjectResult(result);
        }
    }
}