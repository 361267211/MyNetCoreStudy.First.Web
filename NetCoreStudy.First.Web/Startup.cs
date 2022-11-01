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
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);//pg���ݿ���ڵ�ʱ��������Ҫͨ�����д�����
            services.AddDbContext<UserDbContext>(opt =>
            {
                string connStr = "Data Source=.;Initial Catalog=Demo1;User ID=sa;Password=his";
   
                connStr = "User ID=postgres;Password=Pwcwelcome1;Host=localhost;Port=5432;Database=zzqDataBase;Pooling=true;Connection Lifetime=0;";
                opt.UseNpgsql(connStr);
                // opt.UseSqlServer(connStr);
            });

            //�ִ�
            services.AddScoped<IUserDomainRepository, UserDomainRepository>();  

            //�ֲ�ʽ����
            services.AddScoped<IDistributedCacheHelper, DistributedCacheHelper>();

            //Ӧ�÷���
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
                    DateTime expire = DateTime.Now.AddHours(1);//����ʱ���

                    byte[] secBytes = Encoding.UTF8.GetBytes(JwtSettings.SecKey);
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
                });
            services.AddAuthorization();

            //cap
            services.AddCap(c =>
            {
                c.UseEntityFramework<UserDbContext>();
                //User ID=postgres;Password=Pwcwelcome1;Host=localhost;Port=5432;Database=myDataBase;Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0;
                c.UsePostgreSql(connectionString: "User ID=postgres;Password=Pwcwelcome1;Host=localhost;Port=5432;Database=myDataBase;Pooling=true;Connection Lifetime=0;");

                c.FailedRetryCount = 3;//����ʧ�ܻص������Դ�����ע����������Դ�������
                c.FailedRetryInterval = 2;//���Լ��
                c.SucceedMessageExpiredAfter = 60 * 60;//�ɹ���Ϣ�ı���ʱ��
                c.FailedThresholdCallback = async e =>
                 {
                     string message="";
                     foreach (var item in e.Message.Headers)
                     {
                         message+=($"key:{item.Key},value:{item.Value}\r\n");
                     }
                     await System.IO.File.AppendAllTextAsync("d:/error.log", $"ʧ����,������Ϣ\r\n{message}");
                 };
                c.UseRabbitMQ(mq =>
                {
                    mq.HostName = "42.193.20.184"; //RabitMq��������ַ����ʵ������޸Ĵ˵�ַ
                    mq.Port = 5672;
                    mq.UserName = "guest";  //RabbitMq�˺�
                    mq.Password = "guest";  //RabbitMq����
                                            //ָ��Topic exchange���ƣ���ָ���Ļ�����Ĭ�ϵ�
                    mq.ExchangeName = "cap.text.exchange.zzq";//����������

                });

            });

            //������������
            string[] urls = new[] { "http://127.0.0.1:5173" };
            services.AddCors(option =>
            {
                option.AddDefaultPolicy(builder => builder.WithOrigins(urls)
                .AllowAnyMethod().AllowAnyHeader().AllowCredentials()
                );
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
