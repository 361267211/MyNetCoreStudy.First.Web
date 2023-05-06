using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLibrary.Core.Ocelot.JWT
{
    /// <summary>
    /// 权限授权Handler
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        /// <summary>
        /// 验证方案提供对象
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="schemes"></param>
        public PermissionHandler(IAuthenticationSchemeProvider schemes)
        {
            Schemes = schemes;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            //调试时可跳过验证
            if (requirement.OpenJWT != "True")
            {
                context.Succeed(requirement);
                return;
            }

            //从AuthorizationHandlerContext转成HttpContext，以便取出表求信息
            var httpContext = (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext).HttpContext;
            //请求Url
            var questUrl = httpContext.Request.Path.Value.ToLower();
            //判断请求是否停止
            var handlers = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())
            {
                var handler = await handlers.GetHandlerAsync(httpContext, scheme.Name) as IAuthenticationRequestHandler;
                if (handler != null && await handler.HandleRequestAsync())
                {
                    context.Fail();
                    return;
                }
            }
            //判断请求是否拥有凭据，即有没有登录
            var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
            if (defaultAuthenticate != null)
            {
                var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);
                //result?.Principal不为空即登录成功
                if (result?.Principal != null)
                {
                    httpContext.User = result.Principal;

                    //var jwtToken = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                    //var playload = JsonConvert.DeserializeObject<dynamic>(EncrypHelper.DeCodeBase64("utf-8", jwtToken.Split('.')[1]));
                    //string identity = playload["identity"];
                    //string clientType = playload["clientType"];
                    //string username = playload["userName"];
                    //double exp = playload["exp"];
                    string userid = "";
                    string clientType = "";
                    string username = "";
                    double exp = 0;
                    string isAdmin = "";
                    foreach (var a in httpContext.User.Claims)
                    {
                        switch (a.Type)
                        {
                            case "userid":
                                userid = a.Value;
                                break;
                            case "clientType":
                                clientType = a.Value;
                                break;
                            case "userName":
                                username = a.Value;
                                break;
                            case "exp":
                                exp = Convert.ToDouble(a.Value);
                                break;
                            case "isAdmin":
                                isAdmin = a.Value;
                                break;
                            default:
                                break;
                        }
                    }


                    System.DateTime startTime = Convert.ToDateTime(new System.DateTime(1970, 1, 1)); // 当地时区
                    DateTime expDate = startTime.AddSeconds(exp).AddHours(8);

                    if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(clientType))
                    {
                        context.Fail();
                        return;
                    }

                    //根据clientType判断是否需要校验url权限  saas平台的需要校验url权限 
                    //todo 根据自己的权限逻辑编写验证代码  

                    //判断过期时间  
                    if (expDate >= DateTime.Now)
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                    return;
                }
            }
            //判断没有登录时，是否访问登录的url,并且是Post请求，并且是form表单提交类型，否则为失败
            if (!questUrl.Equals(requirement.LoginPath.ToLower(), StringComparison.Ordinal) && (!httpContext.Request.Method.Equals("POST")
            || !httpContext.Request.HasFormContentType))
            {
                context.Fail();
                return;
            }
            context.Succeed(requirement);
        }
    }
}
