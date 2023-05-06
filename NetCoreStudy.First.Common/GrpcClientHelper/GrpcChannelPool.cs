/*********************************************************
* 名    称：GrpcChannelPool.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210827
* 描    述：grpc信道池，使用缓存重用信道
* 更新历史：
*
* *******************************************************/
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.Core.GrpcClientHelper
{
    /// <summary>
    /// Grpc信道池子，用于信道缓存重用，2小时滑动过期
    /// </summary>
    public class GrpcChannelPool
    {
        /// <summary>
        /// 缓存中的信道
        /// </summary>
        private readonly MemoryCache _channels = new MemoryCache(Options.Create(new MemoryCacheOptions()));

        /// <summary>
        /// 同步锁
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// 初始化
        /// </summary>
        public GrpcChannelPool()
        {
        }

        /// <summary>
        /// 通过端点获取信道，如果有缓存则从缓存中获取，key为endpoint
        /// </summary>
        /// <param name="endpoint">端点地址</param>
        /// <returns></returns>
        public Channel GetChannel(Uri endpoint)
        {
            try
            {
                GrpcChanelChecker.CheckNotNull(endpoint, nameof(endpoint));
                return GetChannelFromDict(endpoint, ChannelCredentials.Insecure);
            }
            catch (AggregateException e)
            {
                throw e.InnerExceptions.FirstOrDefault() ?? e;
            }
        }

        /// <summary>
        /// 通过端点获取信道，如果有缓存则从缓存中获取，key为endpoint
        /// </summary>
        /// <param name="endpoint">端点地址</param>
        /// <param name="credentials">证书信息</param>
        /// <returns></returns>
        public Channel GetChannel(Uri endpoint, ChannelCredentials credentials)
        {
            try
            {
                GrpcChanelChecker.CheckNotNull(endpoint, nameof(endpoint));
                return GetChannelFromDict(endpoint, credentials);
            }
            catch (AggregateException e)
            {
                throw e.InnerExceptions.FirstOrDefault() ?? e;
            }
        }

        /// <summary>
        ///  通过端点获取信道，如果有缓存则从缓存中获取，key为endpoint
        /// </summary>
        /// <param name="endpoint">端点地址</param>
        /// <param name="credentials">证书信息</param>
        /// <returns></returns>
        public async Task<Channel> GetChannelAsync(Uri endpoint, ChannelCredentials credentials)
        {
            try
            {
                return await Task.Run(() =>
                {
                    GrpcChanelChecker.CheckNotNull(endpoint, nameof(endpoint));
                    GrpcChanelChecker.CheckNotNull(credentials, nameof(credentials));
                    return GetChannelFromDict(endpoint, credentials);
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///  通过端点获取信道，如果有缓存则从缓存中获取，key为endpoint
        /// </summary>
        /// <param name="endpoint">端点地址</param>
        /// <returns></returns>
        public async Task<Channel> GetChannelAsync(Uri endpoint)
        {
            try
            {
                return await Task.Run(() =>
                {
                    GrpcChanelChecker.CheckNotNull(endpoint, nameof(endpoint));
                    return GetChannelFromDict(endpoint, ChannelCredentials.Insecure);
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 通过端点获取信道，如果有缓存则从缓存中获取，key为endpoint
        /// </summary>
        /// <param name="endpoint">端点地址</param>
        /// <param name="credentials">证书信息</param>
        /// <returns></returns>
        private Channel GetChannelFromDict(Uri endpoint, ChannelCredentials credentials)
        {
            Channel channel = null;
            //尝试取连接信道，没有时新建
            if (!_channels.TryGetValue(endpoint, out channel))
            {
                lock (_lock)
                {
                    //真的没有
                    if (!_channels.TryGetValue(endpoint, out channel))
                    {
                        var options = new[]
                        {
                            //设置维持链接时间:默认60秒
                            new ChannelOption("grpc.keepalive_time_ms", 60_000)
                        };
                        channel = new Channel(endpoint.Host, endpoint.Port, credentials, options);
                        _channels.Set(endpoint, channel, new MemoryCacheEntryOptions
                        {
                            SlidingExpiration = TimeSpan.FromHours(2),//两小时的过期时间
                        });
                    }
                }
            }
            return channel;
        }
    }
}
