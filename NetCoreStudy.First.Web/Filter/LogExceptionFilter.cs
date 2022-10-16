using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Filter
{
    public class LogExceptionFilter : IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            return System.IO.File.AppendAllTextAsync("e:/error.log", context.Exception.Message);
        }
    }
}
 