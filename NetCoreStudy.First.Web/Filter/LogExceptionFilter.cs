using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Filter
{
    public class LogExceptionFilter : IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            return System.IO.File.AppendAllTextAsync("d:/error.log", context.Exception.Message);
        }
    }
}
 