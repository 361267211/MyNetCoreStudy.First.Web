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
            services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordExt>();//�Զ�����Դ����������ģʽ��֤

 


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
            //       .AddDeveloperSigningCredential()  //Ĭ�ϵ����ɵ���Կ�����к󣬻�����Ŀ��Ŀ¼�������ļ� tempkey.jwk��
            //      // .AddInMemoryClients(Config.Clients) //ע��ͻ���
            //     //  .AddInMemoryApiScopes(Config.ApiScopes) //ע��api���ʷ�Χ
            //       .AddAspNetIdentity<MyUser>()
            //    //   .AddTestUsers(Config.Users) //ע����Դӵ����
            //     //  .AddInMemoryIdentityResources(Config.IdentityResources) //�û��������Դ��Ϣ�����磺��ʾ�ǳƣ�ͷ�񣬵ȵ���Ϣ��
            //       ; 






            //EF
            //todo�����ݿ����������ã���ʱ����
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);//pg���ݿ���ڵ�ʱ��������Ҫͨ�����д�����
            services.AddDbContext<UserDbContext>(opt =>
            {
                opt.UseNpgsql(GlobalConfigOption.DbContext.DbConnection);
                opt.UseNpgsql("https://localhost:5001");
            });

            /*            //id4
                        services.AddScoped<UserManager<IdentityUser>>();
                        services.AddScoped<RoleManager<IdentityRole>>();
                         */
            //�ִ�
            services.AddScoped<IUserDomainRepository, UserDomainRepository>();

            //�ֲ�ʽ����
            services.AddScoped<IDistributedCacheHelper, DistributedCacheHelper>();

            //Ӧ�÷���
            services.AddScoped<UserDomainService>();



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

            // services.AddAuthorization();

            //cap
            //todo:��ʱ����
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
                option.Configuration = "localhost";
                option.InstanceName = "zzq_";
            });

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


            //���identityServic4
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

            //    app.UseMiddleware<CheckAndParsingMiddleware>();


            app.UseHttpsRedirection();


            // CAP
            //app.UseCap();

            //app.UseIdentityServer();//ʹ��IdentityServer�м��

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
