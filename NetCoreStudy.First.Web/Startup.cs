using AspNet.Security.OAuth.Validation;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FxCode.FxDatabaseAccessor;
using IdentityServer.EFCore.Entity;
using IdentityServer4.Validation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NetCoreStudy.First.BasicModel;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.Utility.DistributedCache;
using NetCoreStudy.First.Web.AutofacIOC;
using NetCoreStudy.First.Web.Filter;
using NetCoreStudy.First.Web.FxRepository.FxUser;
using NetCoreStudy.First.Web.FxRepository.FxUser;
using NetCoreStudy.First.Web.Middleware;
using NetCoreStudy.First.Web.SignalR;
using StackExchange.Redis;
using System;
using System.IO;
using System.Reflection;

namespace NetCoreStudy.First.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {

            // Register your own things directly with Autofac here. Don't
            // call builder.Populate(), that happens in AutofacServiceProviderFactory
            // for you.
            builder.RegisterModule(new FxAutofacModule());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //ID4
            //    var migrationsAssembly = typeof(UserDbContext).GetTypeInfo().Assembly.GetName().Name;
            services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordExt>();//自定义资源所有者密码模式认证

            services.AddHttpContextAccessor();

            services.AddDirectoryBrowser();

            services.AddDatabaseAccessor();

           // services.Configure<MvcOptions>(e => e.Filters.Add<UnitOfWorkAttribute>());

            services.AddIdentityCore<MyUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            });
            

            services.AddIdentity<MyUser, MyRole>()
                     .AddEntityFrameworkStores<ApplicationDbContext>()
                     .AddDefaultTokenProviders()
                     .AddRoleManager<RoleManager<MyRole>>()
                     .AddUserManager<CustomUserManager<MyUser>>()
                     .AddSignInManager<SignInManager<MyUser>>()
                     .AddUserStore<CustomUserStore<MyUser>>()
                     ;

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



            services.RegisterMapsterConfiguration();

            //EF
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);//pg数据库存在的时区问题需要通过本行代码解决
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseNpgsql(GlobalConfigOption.DbContext.DbConnection);
            });

            services.AddDbContext<FondDbContext>(opt =>
            {
                opt.UseNpgsql(GlobalConfigOption.FondDbContext.DbConnection);
            });

            //id4
            services.AddScoped<MyUser>();

            services.AddScoped(typeof(IUserStore<>), typeof(CustomUserStore<>));
            services.AddScoped<CustomUserManager<MyUser>>();

            //仓储 杨中科的示例代码
            //services.AddScoped<IUserDomainRepository, UserDomainRepository>();

            //注册分布式缓存帮助类
            services.AddScoped<IDistributedCacheHelper, DistributedCacheHelper>();

            //用户管理的注册
            //services.AddScoped<IUserManagerService, UserManagerService>();
            //services.AddScoped<IUserManagerRepository, UserManagerRepository>();



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

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = OAuthValidationConstants.Schemes.Bearer;
            })
                .AddJwtBearer("Bearer", config =>
                {
                    config.Authority = "https://localhost:5001";
                    config.Audience = "api1";


                });

            services.AddAuthorization();

            //刘孟配置的cap
            //todo:暂时禁用，CAP 可用后解除注释
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


            //felix 配置的CAP
            services.AddCap(x =>
            {
                x.CollectorCleaningInterval = 300;
                x.ConsumerThreadCount = 1;
                x.FailedRetryCount = 50;
                x.FailedRetryInterval = 60;
                x.UseStorageLock = true;
                x.UseDashboard();
                x.UseRabbitMQ(configure: RabOpt =>
                {
                    RabOpt.UserName = "guest";
                    RabOpt.Password = "guest";
                    RabOpt.HostName = "42.193.20.184";
                    RabOpt.Port = 5672;
                    RabOpt.ExchangeName = "cap.felix.exchange";

                });
                x.UsePostgreSql(configure: PgOpt =>
                {
                    PgOpt.ConnectionString = "Host=42.193.20.184;Port=5432;User ID=postgres;Password=Pwcwelcome1;Database=felix_Fond;Pooling=true;";
                });
            });

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
                option.Configuration = "42.193.20.184:6379,abortConnect=false,password=Pwcwelcome1";
            });
            services.AddScoped<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect("42.193.20.184:6379,abortConnect=false,password=Pwcwelcome1"));

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

            //注册默认容器之前注册的类
            var builder = new ContainerBuilder();
            builder.Populate(services);

            services.AddControllers();

            //注册swagger的信息
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
            app.UseStaticFiles();
            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\uploads")),
                RequestPath = new PathString("/Uploads"),
                EnableDirectoryBrowsing = true
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NetCoreStudy.First.Web v1"));
            }

            //自定义中间件使用代码示例
            //app.UseMiddleware<FxTemplateMiddleware>();

            //重定向
            app.UseHttpsRedirection();


            // 使用CAP，需配合configeservice中的部分一起使用
            //app.UseCap();


            app.UseRouting();

            //使用跨域策略，需配合configeservice中的部分一起使用
            app.UseCors("apiPolicy");

            //使用 鉴权
            app.UseAuthentication();


            //使用 授权
            app.UseAuthorization();

            //使用终结点路由
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MyHub>("/MyHub");
            });
        }
    }
}
