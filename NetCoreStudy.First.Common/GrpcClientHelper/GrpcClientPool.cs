/*********************************************************
* 名    称：GrpcClientPool.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210827
* 描    述：grpc客户对象，使用缓存管理
* 更新历史：
*
* *******************************************************/
using Furion.FriendlyException;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;

namespace NetCoreStudy.Core.GrpcClientHelper
{
    /// <summary>
    /// Grpc客户端池子，用于客户端缓存重用，2小时滑动过期
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GrpcClientPool<T> where T : ClientBase<T>
    {
        /// <summary>
        /// 信道池子
        /// </summary>
        private GrpcChannelPool _chanelPool { get; set; }
        /// <summary>
        /// 客户端池子
        /// </summary>
        private readonly MemoryCache _clients = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        /// <summary>
        /// 同步锁
        /// </summary>
        private readonly object _lock = new object();
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="chanelPool"></param>
        public GrpcClientPool(GrpcChannelPool chanelPool)
        {
            _chanelPool = chanelPool;
        }
        /// <summary>
        /// 通过地址获取GrpcClient对象
        /// </summary>
        /// <param name="endpoint">端点地址</param>
        /// <returns></returns>
        public T GetClient(Uri endpoint)
        {
            try
            {
                GrpcChanelChecker.CheckNotNull(endpoint, nameof(endpoint));
                return GetClientFromDict(endpoint);

            }
            catch (Exception e)
            {
                throw Oops.Oh(e.Message);
            }

        }
        /// <summary>
        /// 通过地址获取GrpcClient对象
        /// </summary>
        /// <param name="endpoint">端点地址</param>
        /// <returns></returns>
        private T GetClientFromDict(Uri endpoint)
        {
            T client = default(T);
            if (!_clients.TryGetValue<T>(endpoint, out client))
            {
                lock (_lock)
                {
                    if (!_clients.TryGetValue<T>(endpoint, out client))
                    {
                        var chanel = _chanelPool.GetChannel(endpoint);
                        client = (T)Activator.CreateInstance(typeof(T), new object[] { chanel });
                        _clients.Set(endpoint, client, new MemoryCacheEntryOptions
                        {
                            SlidingExpiration = TimeSpan.FromHours(2),
                        });
                    }
                }
            }
            return client;
        }
    }
}
