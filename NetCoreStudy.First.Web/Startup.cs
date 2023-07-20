using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityServer.EFCore.Entity;
using IdentityServer4.Validation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NetCoreStudy.First.BasicModel;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.Utility.DistributedCache;
using NetCoreStudy.First.Web.AutofacIOC;
using NetCoreStudy.First.Web.FxRepository.FxUser;
using NetCoreStudy.First.Web.FxRepository.FxUser;
using NetCoreStudy.First.Web.Middleware;
using NetCoreStudy.First.Web.SignalR;
using StackExchange.Redis;
using System;
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
            services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordExt>();//�Զ�����Դ����������ģʽ��֤

            services.AddHttpContextAccessor();


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
            ;

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
            //       .AddDeveloperSigningCredential()  //Ĭ�ϵ����ɵ���Կ�����к󣬻�����Ŀ��Ŀ¼�������ļ� tempkey.jwk��
            //      // .AddInMemoryClients(Config.Clients) //ע��ͻ���
            //     //  .AddInMemoryApiScopes(Config.ApiScopes) //ע��api���ʷ�Χ
            //       .AddAspNetIdentity<MyUser>()
            //    //   .AddTestUsers(Config.Users) //ע����Դӵ����
            //     //  .AddInMemoryIdentityResources(Config.IdentityResources) //�û��������Դ��Ϣ�����磺��ʾ�ǳƣ�ͷ�񣬵ȵ���Ϣ��
            //       ; 






            //EF
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);//pg���ݿ���ڵ�ʱ��������Ҫͨ�����д�����
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

            //�ִ� ���пƵ�ʾ������
            //services.AddScoped<IUserDomainRepository, UserDomainRepository>();

            //ע��ֲ�ʽ���������
            services.AddScoped<IDistributedCacheHelper, DistributedCacheHelper>();

            //�û������ע��
            //services.AddScoped<IUserManagerService, UserManagerService>();
            //services.AddScoped<IUserManagerRepository, UserManagerRepository>();



            //JWT
            //services.Configure<JWTSettings>(Configuration.GetSection("JWT"));
            /* services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(opt =>
                 {

                     byte[] secBytes = Encoding.UTF8.GetBytes(GlobalConfigOption.JwtAuth.SecKey);
                     var secKey = new SymmetricSecurityKey(secBytes);//��Կ

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

             services.AddAuthorization();

            //cap
            //todo:��ʱ���ã�CAP ���ú���ע��
            {
                /*
                                services.AddCap(c =>
                                {
                                    DotNetCore.CAP.CapOptions capOptions = c.UseEntityFramework<UserDbContext>();

                                    c.UsePostgreSql(connectionString: GlobalConfigOption.DbContext.DbConnection);

                                    c.FailedRetryCount = 3;//����ʧ�ܻص������Դ�����ע����������Դ�������
                                    c.FailedRetryInterval = 2;//���Լ��
                                    c.SucceedMessageExpiredAfter = 60 * 60;//�ɹ���Ϣ�ı���ʱ��
                                    c.FailedThresholdCallback = async e =>
                                    {
                                        string message = "";
                                        foreach (var item in e.Message.Headers)
                                        {
                                            message += ($"key:{item.Key},value:{item.Value}\r\n");
                                        }
                                        await System.IO.File.AppendAllTextAsync("d:/error.log", $"ʧ����,������Ϣ\r\n{message}");
                                    };
                                    c.UseRabbitMQ(mq =>
                                    {
                                        mq.HostName = GlobalConfigOption.Cap.RabbitMQ.HostName; //RabitMq��������ַ����ʵ������޸Ĵ˵�ַ
                                        mq.Port = GlobalConfigOption.Cap.RabbitMQ.Port;
                                        mq.UserName = GlobalConfigOption.Cap.RabbitMQ.UserName;  //RabbitMq�˺�
                                        mq.Password = GlobalConfigOption.Cap.RabbitMQ.Password;  //RabbitMq����
                                                                                                 //ָ��Topic exchange���ƣ���ָ���Ļ�����Ĭ�ϵ�
                                        mq.ExchangeName = "cap.text.exchange.zzq";//����������

                                    });
                                });
                */
            }


            //������������
            services.AddCors(options =>
            {
                //ע�⣺�Զ���Ŀ����������,�����Configure�е�UseCors�����Ʊ���һ��
                options.AddPolicy("apiPolicy", policy =>
                {
                    //1.����������Դ�����������Դʱ����ʹ��, �ָ policy.WithOrigins("https://127.0.0.1:5003","https://127.0.0.1:7001")
                    //2.���������Դʱ����ʹ��string[]
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
            //����redis��Ϊ�ֲ�ʽ����
            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = "42.193.20.184:6379,abortConnect=false,password=Pwcwelcome1";
            });
            services.AddScoped<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect("42.193.20.184:6379,abortConnect=false,password=Pwcwelcome1"));

            //����SignalR��webSocket
            services.AddSignalR();

            //ע���¼���ʿMediatR
            var refAssembyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            foreach (var asslembyNames in refAssembyNames)
            {
                Assembly.Load(asslembyNames);
            }
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            services.AddMediatR(assemblies);

            //ע��Ĭ������֮ǰע�����
            var builder = new ContainerBuilder();
            builder.Populate(services);

            services.AddControllers();

            //ע��swagger����Ϣ
            services.AddSwaggerGen(c =>
            {
                //Bearer ��scheme����
                var securityScheme = new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    //���������ͷ��
                    In = ParameterLocation.Header,
                    //ʹ��Authorizeͷ��
                    Type = SecuritySchemeType.Http,
                    //����Ϊ�� bearer��ͷ
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                };

                //�����з�������Ϊ����bearerͷ����Ϣ
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

                //ע�ᵽswagger��
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

            //�Զ����м��ʹ�ô���ʾ��
            //app.UseMiddleware<FxTemplateMiddleware>();

            //�ض���
            app.UseHttpsRedirection();


            // ʹ��CAP�������configeservice�еĲ���һ��ʹ��
            //app.UseCap();


            app.UseRouting();

            //ʹ�ÿ�����ԣ������configeservice�еĲ���һ��ʹ��
            app.UseCors("apiPolicy");

            //ʹ�� ��Ȩ
            app.UseAuthentication();


            //ʹ�� ��Ȩ
            app.UseAuthorization();

            //ʹ���ս��·��
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MyHub>("/MyHub");
            });
        }
    }
}
