/*********************************************************
* 名    称：SmartConsulBuilder.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210826
* 描    述：注册具体实现，随机选取配置的Consul端点的一半地址，向其进行服务注册，实现高可用
* 更新历史：
*
* *******************************************************/
using Consul;
//using Furion.FriendlyException;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreStudy.Core.Consul
{
    /// <summary>
    /// 智图Consul客户端连接构造器
    /// </summary>
    public class SmartConsulBuilder
    {
        /// <summary>
        /// 心跳检查信息
        /// </summary>
        private readonly List<AgentServiceCheck> _checks = new List<AgentServiceCheck>();
        /// <summary>
        /// 需要连接注册服务的客户端
        /// </summary>
        private readonly List<ConsulClient> _registryClients = new List<ConsulClient>();
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="addr"></param>
        public SmartConsulBuilder(string addr)
        {
            InitRegistryHalfClients(addr);
        }
        /// <summary>
        /// 初始话Consul客户端连接信息
        /// </summary>
        /// <param name="consulAddress"></param>
        private void InitRegistryHalfClients(string consulAddress)
        {
            var consulAddrList = consulAddress.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();
            if (consulAddrList == null || !consulAddrList.Any())
            {
                //throw Oops.Oh("未找到Consul节点");
            }
            var allNodes = consulAddrList;
            var hafCount = (int)Math.Ceiling(allNodes.Count() / 2m);
            var rd = new Random();
            var sIndex = rd.Next(1, allNodes.Count());
            for (var i = 0; i < hafCount; i++)
            {
                var cIndex = (sIndex + i) % allNodes.Count();
                var cNode = allNodes[cIndex];
                ConsulClient client = new ConsulClient(c =>
                {
                    c.Address = new Uri(cNode);
                });
                _registryClients.Add(client);
            }
        }

        /// <summary>
        /// 添加心跳检查信息
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public SmartConsulBuilder AddHealthCheck(AgentServiceCheck check)
        {
            _checks.Add(check);
            return this;
        }


        /// <summary>
        /// 添加Http类型心跳检查信息
        /// </summary>
        /// <param name="url">端点地址</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="interval">间隔时间</param>
        /// <returns></returns>
        public SmartConsulBuilder AddHttpHealthCheck(string url, int timeout = 10, int interval = 10)
        {
            _checks.Add(new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(60),
                Interval = TimeSpan.FromSeconds(interval),
                HTTP = url,
                Timeout = TimeSpan.FromSeconds(timeout),
            });
            return this;
        }

        /// <summary>
        /// 添加Grpc类型心跳检查信息
        /// </summary>
        /// <param name="endpoint">端点地址</param>
        /// <param name="grpcUseTls">是否使用Tls</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="interval">间隔时间</param>
        /// <returns></returns>
        public SmartConsulBuilder AddGRPCHealthCheck(string endpoint, bool grpcUseTls = false, int timeout = 10, int interval = 10)
        {
            _checks.Add(new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(60),
                Interval = TimeSpan.FromSeconds(interval),
                GRPC = endpoint,
                GRPCUseTLS = grpcUseTls,
                Timeout = TimeSpan.FromSeconds(timeout)
            });
            return this;
        }

        /// <summary>
        /// 向Consul注册服务
        /// </summary>
        /// <param name="name"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="tags"></param>
        public void RegisterService(string name, string host, int port, string[] tags)
        {
            var registration = new AgentServiceRegistration()
            {
                Checks = _checks.ToArray(),
                ID = $"{name}_node_{host}:{port}",
                Name = name,
                Address = host,
                Port = port,
                Tags = tags,
            };
            _registryClients.ForEach(client =>
            {
                client.Agent.ServiceRegister(registration).Wait();
            });

        }

    }
}
