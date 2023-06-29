using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Filter
{
    public class MyExceptionFilter : IAsyncExceptionFilter
    {
        private readonly IWebHostEnvironment _hostEnv;
        public MyExceptionFilter(IWebHostEnvironment hostEnv)
        {
            _hostEnv = hostEnv;
        }
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            string msg;
            if (_hostEnv.IsDevelopment())
            {
                msg = context.Exception.Message;
            }
            else
            {
                msg = "服务器发生未处理的异常";
            }

            ObjectResult objectResult = new ObjectResult(new { code = 500, message = msg });
            context.Result = objectResult;
        }
    }
}
