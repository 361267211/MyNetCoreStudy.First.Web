using IdentityServer.EFCore.Entity;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Test;
using IdentityServer4.Validation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetCoreStudy.First.BasicModel;
using NetCoreStudy.First.Domain;
using NetCoreStudy.First.Domain.Entity;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.Utility.DistributedCache;
using NetCoreStudy.First.Web.Filter;
using NetCoreStudy.First.Web.IdentityService4;
using NetCoreStudy.First.Web.JWT;
using NetCoreStudy.First.Web.Middleware;
using NetCoreStudy.First.Web.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //ID4
            var migrationsAssembly = typeof(UserDbContext).GetTypeInfo().Assembly.GetName().Name;
            services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordExt>();//自定义资源所有者密码模式认证

 


            //services.AddIdentityCore<MyUser>(options =>
            //{
            //    options.Password.RequireDigit = false;
            //    options.Password.RequireLowercase = false;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequiredLength = 6;
            //    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            //    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            //});
;

            //services.AddIdentity<MyUser, MyRole>()
            //         .AddEntityFrameworkStores<ApplicationDbContext>()
            //         .AddDefaultTokenProviders()
            //         .AddRoleManager<RoleManager<MyRole>>()
            //         .AddUserManager<UserManager<MyUser>>()
            //         .AddSignInManager<SignInManager<MyUser>>()
            //         ;

            //services.AddIdentityServer().AddConfigurationStore(options =>
            //       {
            //           options.ConfigureDbContext = b => b.UseNpgsql(GlobalConfigOption.DbContext.DbConnection,
            //               sql => sql.MigrationsAssembly(migrationsAssembly));
            //       })
            //       .AddOperationalStore(options =>
            //       {
            //           options.ConfigureDbContext = b => b.UseNpgsql(GlobalConfigOption.DbContext.DbConnection,
            //               sql => sql.MigrationsAssembly(migrationsAssembly));
            //       })
            //       .AddDeveloperSigningCredential()  //默认的生成的密钥（运行后，会在项目根目录下生成文件 tempkey.jwk）
            //      // .AddInMemoryClients(Config.Clients) //注册客户端
            //     //  .AddInMemoryApiScopes(Config.ApiScopes) //注册api访问范围
            //       .AddAspNetIdentity<MyUser>()
            //    //   .AddTestUsers(Config.Users) //注册资源拥有者
            //     //  .AddInMemoryIdentityResources(Config.IdentityResources) //用户的身份资源信息（例如：显示昵称，头像，等等信息）
            //       ; 






            //EF
            //todo：数据库上下文配置，暂时禁用
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);//pg数据库存在的时区问题需要通过本行代码解决
            services.AddDbContext<UserDbContext>(opt =>
            {
                opt.UseNpgsql(GlobalConfigOption.DbContext.DbConnection);
                opt.UseNpgsql("https://localhost:5001");
            });

            /*            //id4
                        services.AddScoped<UserManager<IdentityUser>>();
                        services.AddScoped<RoleManager<IdentityRole>>();
                         */
            //仓储
            services.AddScoped<IUserDomainRepository, UserDomainRepository>();

            //分布式缓存
            services.AddScoped<IDistributedCacheHelper, DistributedCacheHelper>();

            //应用服务
            services.AddScoped<UserDomainService>();



            //JWT
            //services.Configure<JWTSettings>(Configuration.GetSection("JWT"));
            /* services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(opt =>
                 {

                     byte[] secBytes = Encoding.UTF8.GetBytes(GlobalConfigOption.JwtAuth.SecKey);
                     var secKey = new SymmetricSecurityKey(secBytes);//密钥

                     opt.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = false,
                         ValidateAudience = false,
                         ValidateLifetime = true,
                         IssuerSigningKey = secKey
                     };

                     opt.Events = new JwtBearerEvents
                     {
                         OnMessageReceived = context =>
                         {
                             var accessToken = context.Request.Query["access_token"];
                             var path = context.Request.Path;
                             if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/MyHub"))
                             {
                                 context.Token = accessToken;
                             }
                             return Task.CompletedTask;
                         }
                     };
                 });*/

             services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", config =>
                {
                    config.Authority = "https://localhost:5001";
                    config.Audience = "api1";


                });

            // services.AddAuthorization();

            //cap
            //todo:暂时禁用
            {
/*
                services.AddCap(c =>
                {
                    DotNetCore.CAP.CapOptions capOptions = c.UseEntityFramework<UserDbContext>();

                    c.UsePostgreSql(connectionString: GlobalConfigOption.DbContext.DbConnection);

                    c.FailedRetryCount = 3;//触发失败回调的重试次数，注意与最大重试次数区分
                    c.FailedRetryInterval = 2;//重试间隔
                    c.SucceedMessageExpiredAfter = 60 * 60;//成功消息的保存时间
                    c.FailedThresholdCallback = async e =>
                    {
                        string message = "";
                        foreach (var item in e.Message.Headers)
                        {
                            message += ($"key:{item.Key},value:{item.Value}\r\n");
                        }
                        await System.IO.File.AppendAllTextAsync("d:/error.log", $"失败了,报错信息\r\n{message}");
                    };
                    c.UseRabbitMQ(mq =>
                    {
                        mq.HostName = GlobalConfigOption.Cap.RabbitMQ.HostName; //RabitMq服务器地址，依实际情况修改此地址
                        mq.Port = GlobalConfigOption.Cap.RabbitMQ.Port;
                        mq.UserName = GlobalConfigOption.Cap.RabbitMQ.UserName;  //RabbitMq账号
                        mq.Password = GlobalConfigOption.Cap.RabbitMQ.Password;  //RabbitMq密码
                                                                                 //指定Topic exchange名称，不指定的话会用默认的
                        mq.ExchangeName = "cap.text.exchange.zzq";//交换的名称

                    });
                });
*/
            }
             

            //跨域问题配置
            services.AddCors(options =>
            {
                //注意：自定义的跨域策略名称,必须和Configure中的UseCors的名称必须一致
                options.AddPolicy("apiPolicy", policy =>
                {
                    //1.允许跨域的来源，多个跨域来源时可以使用, 分割， policy.WithOrigins("https://127.0.0.1:5003","https://127.0.0.1:7001")
                    //2.多个跨域来源时可以使用string[]
                    string Origins = "http://127.0.0.1:5003";

                    //policy.WithOrigins(Origins.Split(','))
                    policy.
                    AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(_ => true)
                    .AllowCredentials();
                    ;
                });
            });
            //加入redis作为分布式缓存
            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = "localhost";
                option.InstanceName = "zzq_";
            });

            //加入SignalR，webSocket
            services.AddSignalR();

            //注册事件巴士MediatR
            var refAssembyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            foreach (var asslembyNames in refAssembyNames)
            {
                Assembly.Load(asslembyNames);
            }
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            services.AddMediatR(assemblies);


            //添加identityServic4
            /*            services.AddIdentityServer()
                            .AddDeveloperSigningCredential()
                            // .AddInMemoryIdentityResources(IdentityConfig.IdentityResources)
                             .AddInMemoryApiScopes(Config.GetApiScopes())
                            .AddInMemoryClients(Config.GetClients())
                            .AddInMemoryApiResources(Config.GetApiResources())
                            //  .AddTestUsers(IdentityConfig.GetTestUsers().ToList())
                            ;*/





            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                //Bearer 的scheme定义
                var securityScheme = new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    //参数添加在头部
                    In = ParameterLocation.Header,
                    //使用Authorize头部
                    Type = SecuritySchemeType.Http,
                    //内容为以 bearer开头
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                };

                //把所有方法配置为增加bearer头部信息
                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "bearerAuth"
                                }
                            },
                            new string[] {}
                    }
                };

                //注册到swagger中
                c.AddSecurityDefinition("bearerAuth", securityScheme);
                c.AddSecurityRequirement(securityRequirement);
            });
        
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           // app.UseDatabaseInitialize();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NetCoreStudy.First.Web v1"));
            }

            //    app.UseMiddleware<CheckAndParsingMiddleware>();


            app.UseHttpsRedirection();


            // CAP
            //app.UseCap();

            //app.UseIdentityServer();//使用IdentityServer中间件

            app.UseRouting();

            app.UseCors("apiPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MyHub>("/MyHub");
            });
        }
    }
}
