using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Middleware
{
    public class CheckAndParsingMiddleware
    {
        private readonly RequestDelegate next;
        public CheckAndParsingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await context.Response.WriteAsync("CheckAndParsingMiddleware start<br/>");
            await next.Invoke(context);
            await context.Response.WriteAsync("CheckAndParsingMiddleware end<br/>");
        }
    }
}
