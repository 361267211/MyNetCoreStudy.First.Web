/*********************************************************
* 名    称：GrpcClientFactory.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210827
* 描    述：Grpc客户端对象工厂，用户管理grpc客户端对象
* 更新历史：
*
* *******************************************************/
using Furion.FriendlyException;
using Grpc.Core;
using System;

namespace NetCoreStudy.Core.GrpcClientHelper
{
    /// <summary>
    /// Grpc服务客户端工厂，通过缓存重用客户端与信道
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SmartGrpcClientFactory<T> where T : ClientBase<T>
    {
        /// <summary>
        /// 客户端池子
        /// </summary>
        private GrpcClientPool<T> _clientPool { get; set; }
        /// <summary>
        /// grpc服务地址获取器
        /// </summary>
        private IGrpcTargetAddressResolver _grpcTargetResolver { get; set; }

        /// <summary>
        /// 服务初始化
        /// </summary>
        /// <param name="clientPool"></param>
        /// <param name="grpcTargetResolver"></param>
        public SmartGrpcClientFactory(GrpcClientPool<T> clientPool,
            IGrpcTargetAddressResolver grpcTargetResolver)
        {
            _clientPool = clientPool;
            _grpcTargetResolver = grpcTargetResolver;
        }
        /// <summary>
        /// 通过OrgCode获取客户端对象
        /// 通过OrgCode获取服务地址，再根据服务地址从缓存中获取Client对象
        /// </summary>
        /// <param name="orgCode"></param>
        /// <returns></returns>
        public T GetGrpcClient(string orgCode)
        {
            var targetAddress = _grpcTargetResolver.GetGrpcTargetAddress(orgCode);
            if (!GrpcChanelChecker.CheckIsUrlFormat(targetAddress))
            {
                throw Oops.Oh("信道地址不合法");
            }
            var uriAddress = new Uri(targetAddress);
            var tenantChanel = _clientPool.GetClient(uriAddress);
            return tenantChanel;
        }



    }
}
