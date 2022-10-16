using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

namespace NetCoreStudy.First.Web.Filter
{
    public class TransactionScopeFilter : IAsyncActionFilter
    {

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool hasNotTransactionalAttribute = false;
            if (context.ActionDescriptor is ControllerActionDescriptor)
            {
                var actionDesc = (ControllerActionDescriptor)context.ActionDescriptor;
                hasNotTransactionalAttribute = actionDesc.MethodInfo.IsDefined(typeof(NotTransactionalAttribute));
            }

            if (hasNotTransactionalAttribute)
            {
                await next();
                return;
            }

            using var txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var result = await next();
            {
                txScope.Complete();
            }

            
        }
    }
}
