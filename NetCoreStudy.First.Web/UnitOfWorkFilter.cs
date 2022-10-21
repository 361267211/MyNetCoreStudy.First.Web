using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web
{
    public class UnitOfWorkFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await next();
            if (result.Exception != null)
            {
                return;
            }

            var actionDesc = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDesc == null)
            {
                return;
            }

            var attr = actionDesc.MethodInfo.GetCustomAttribute<UnitOfWorkAttribute>();
            if (attr == null)
            {
                return;

            }

            foreach (var dbCTX in attr._dbContextType)
            {
                var DB = context.HttpContext.RequestServices.GetService(dbCTX) as DbContext;
                if (DB != null)
                {
                    await DB.SaveChangesAsync();
                }
            }
        }
    }
}
