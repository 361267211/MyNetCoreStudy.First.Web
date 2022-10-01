using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Filter
{
    public class MyActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Console.WriteLine("前代码");
            ActionExecutedContext result = await next();
            if (result.Exception!=null)
            {
                Console.WriteLine("发生异常");
            }
            else
            {
                Console.WriteLine("核心业务执行成功");

            }
            Console.WriteLine("前代码");
        }
    }
}
