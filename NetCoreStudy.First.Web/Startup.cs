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
using NetCoreStudy.First.Domain;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.Utility.DistributedCache;
using NetCoreStudy.First.Web.Filter;
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
            //EF
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);//pg数据库存在的时区问题需要通过本行代码解决
            services.AddDbContext<UserDbContext>(opt =>
            {
                string connStr = "Data Source=.;Initial Catalog=Demo1;User ID=sa;Password=his";
   
                connStr = "User ID=postgres;Password=Pwcwelcome1;Host=localhost;Port=5432;Database=zzqDataBase;Pooling=true;Connection Lifetime=0;";
                opt.UseNpgsql(connStr);
                // opt.UseSqlServer(connStr);
            });

            //仓储
            services.AddScoped<IUserDomainRepository, UserDomainRepository>();  

            //分布式缓存
            services.AddScoped<IDistributedCacheHelper, DistributedCacheHelper>();

            //应用服务
            services.AddScoped<UserDomainService>();

            services.Configure<MvcOptions>(opt =>
            {
                opt.Filters.Add<MyExceptionFilter>();
                opt.Filters.Add<LogExceptionFilter>();
                opt.Filters.Add<MyActionFilter>();
                opt.Filters.Add<TransactionScopeFilter>();
                opt.Filters.Add<UnitOfWorkFilter>();
            });

            //JWT
            services.Configure<JWTSettings>(Configuration.GetSection("JWT"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    var JwtSettings = Configuration.GetSection("JWT").Get<JWTSettings>();
                    DateTime expire = DateTime.Now.AddHours(1);//过期时间点

                    byte[] secBytes = Encoding.UTF8.GetBytes(JwtSettings.SecKey);
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
                });
            services.AddAuthorization();

            //cap
            services.AddCap(c =>
            {
                c.UseEntityFramework<UserDbContext>();
                //User ID=postgres;Password=Pwcwelcome1;Host=localhost;Port=5432;Database=myDataBase;Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0;
                c.UsePostgreSql(connectionString: "User ID=postgres;Password=Pwcwelcome1;Host=localhost;Port=5432;Database=myDataBase;Pooling=true;Connection Lifetime=0;");

                c.FailedRetryCount = 3;//触发失败回调的重试次数，注意与最大重试次数区分
                c.FailedRetryInterval = 2;//重试间隔
                c.SucceedMessageExpiredAfter = 60 * 60;//成功消息的保存时间
                c.FailedThresholdCallback = async e =>
                 {
                     string message="";
                     foreach (var item in e.Message.Headers)
                     {
                         message+=($"key:{item.Key},value:{item.Value}\r\n");
                     }
                     await System.IO.File.AppendAllTextAsync("d:/error.log", $"失败了,报错信息\r\n{message}");
                 };
                c.UseRabbitMQ(mq =>
                {
                    mq.HostName = "42.193.20.184"; //RabitMq服务器地址，依实际情况修改此地址
                    mq.Port = 5672;
                    mq.UserName = "guest";  //RabbitMq账号
                    mq.Password = "guest";  //RabbitMq密码
                                            //指定Topic exchange名称，不指定的话会用默认的
                    mq.ExchangeName = "cap.text.exchange.zzq";//交换的名称

                });

            });

            //跨域问题配置
            string[] urls = new[] { "http://127.0.0.1:5173" };
            services.AddCors(option =>
            {
                option.AddDefaultPolicy(builder => builder.WithOrigins(urls)
                .AllowAnyMethod().AllowAnyHeader().AllowCredentials()
                );
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


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                var scheme = new OpenApiSecurityScheme()
                {
                    Description = "Authorization header. \r\nExample: 'Bearer 12345abcdef'",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Authorization"
                    },
                    Scheme = "oauth2",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                };
                c.AddSecurityDefinition("Authorization", scheme);
                var requirement = new OpenApiSecurityRequirement();
                requirement[scheme] = new List<string>();
                c.AddSecurityRequirement(requirement);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NetCoreStudy.First.Web v1"));
            }

            //    app.UseMiddleware<CheckAndParsingMiddleware>();

            app.UseCors();

            app.UseHttpsRedirection();


            // CAP
            //app.UseCap();

            app.UseRouting();

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
