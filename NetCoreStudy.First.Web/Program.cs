using ApolloOption;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NetCoreStudy.First.BasicModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Winton.Extensions.Configuration.Consul;

namespace NetCoreStudy.First.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(configureDelegate: (hostBuilderContext, configBuilder) =>
                          {
                              var baseConfig = configBuilder.Build();//configRoot ����������˲ʱ��


                              if (baseConfig["Setup:ConfigType"] == "Consul")
                              {
                                  var configSchema = baseConfig["Setup:ConsulConfigSchema"];
                                  var configAddress = baseConfig["Setup:ConsulConfigAddress"];
                                  //ʹ��consul�ͻ��˼���consul����
                                  configBuilder.AddConsul(configSchema, options =>
                                  {
                                      options.ConsulConfigurationOptions = cco =>
                                      {
                                          cco.Address = new Uri(configAddress);
                                      };
                                      //�����ȸ��� ��̬����
                                      options.ReloadOnChange = true;
                                      options.Optional = true;
                                  });

                                  var finalConfig = configBuilder.Build();//configRoot1 ����������˲ʱ��
                                  OptionRegister.ConsulConfigInit(finalConfig, typeof(GlobalConfigOption));
                                  ChangeToken.OnChange(() => finalConfig.GetReloadToken(), () =>
                                  {
                                      //Console.WriteLine("��������");
                                      OptionRegister.ConsulConfigInit(finalConfig, typeof(GlobalConfigOption));
                                  });


                              }
                              else if (baseConfig["Setup:ConfigType"] == "Apollo")
                              {
                                  // configBuilder.AddApollo(baseConfig.GetSection("Apollo"));
                              }
                          })
            .ConfigureWebHostDefaults(webBuilder =>
            {


                webBuilder.UseStartup<Startup>();

            });
    }
}
