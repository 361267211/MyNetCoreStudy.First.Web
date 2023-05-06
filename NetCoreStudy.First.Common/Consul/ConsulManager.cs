using Consul;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.Core.Consul
{

    /// <summary>
    /// 
    /// </summary>
    public static class ConsulExtension
    {
        public static void ConsulRegist(this IConfiguration configuration)
        {
            try
            {
                var consultConfig = configuration.GetSection("Consul");
                string ip = consultConfig["Url"];
                string port = consultConfig["Port"];
                string weight = consultConfig["Weight"];
                string consulAddress = consultConfig["ConsulAddress"];
                string consulCenter = consultConfig["ConsulCenter"];
                string serviceName = consultConfig["ConsulServiceName"];
                string serviceTags = consultConfig["ConsulServiceTags"];

                ConsulClient client = new ConsulClient(c =>
                {
                    c.Address = new Uri(consulAddress);
                    c.Datacenter = consulCenter;
                });

                client.Agent.ServiceRegister(new AgentServiceRegistration()
                {
                    ID = $"{serviceName}_Node_{ip}:{port}",//--唯一的
                    Name = serviceName,//分组---根据Service
                    Address = ip,
                    Port = int.Parse(port),
                    Tags = serviceTags.Split(','),//额外标签信息
                    Check = new AgentServiceCheck()
                    {
                        Interval = TimeSpan.FromSeconds(12),
                        HTTP = $"http://{ip}:{port}/Api/Health/Index", // 给到200
                        Timeout = TimeSpan.FromSeconds(5),
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(20)
                    }//配置心跳
                });
                Console.WriteLine($"{ip}:{port}--weight:{weight}"); //命令行参数获取
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Consul注册：{ex.Message}");
            }
        }

        /// <summary>
        /// 发现服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="consulUrl"></param>
        /// <returns></returns>
        public static async Task<CatalogService> Discovery(string serviceName, string consulUrl)
        {
            //consul 发现对应的grpc服务

            var consulClient = new ConsulClient(c => c.Address = new Uri(consulUrl));
            var services = await consulClient.Catalog.Service(serviceName);
            if (services.Response.Length == 0)
            {
                throw new Exception($"未发现服务 {serviceName}");
            }
            var service = services.Response[0];

            return service;
        }

    }

}
