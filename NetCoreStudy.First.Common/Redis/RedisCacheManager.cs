/*********************************************************
* 名    称：RedisCacheManager.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210831
* 描    述：Redis缓存管理器
* 更新历史：
*
* *******************************************************/
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Linq;

namespace NetCoreStudy.Core.Redis
{
    /// <summary>
    /// Redis缓存管理器
    /// </summary>
    public class RedisCacheManager : IRedisCacheManager
    {
        /// <summary>
        /// redis缓存配置项
        /// </summary>
        private readonly RedisCacheOptions _redisCacheOptions;
        /// <summary>
        /// redis连接对象
        /// </summary>
        private readonly ConnectionMultiplexer _redisConnection;
        /// <summary>
        /// redis同步锁
        /// </summary>
        private readonly object redisConnectionLock = new object();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="redisCacheOptions"></param>
        public RedisCacheManager(IOptions<RedisCacheOptions> redisCacheOptions)
        {
            _redisCacheOptions = redisCacheOptions.Value;
            if (_redisCacheOptions == null)
            {
                throw new Exception("为获取到配置信息");
            }
            if (string.IsNullOrWhiteSpace(_redisCacheOptions.ConnectAddress))
            {
                throw new Exception("未配置连接信息");
            }
            this._redisConnection = GetRedisConnection();
        }

        /// <summary>
        /// 获取redisConnection对象单例
        /// </summary>
        /// <returns></returns>
        private ConnectionMultiplexer GetRedisConnection()
        {
            //如果已经连接实例，直接返回
            if (this._redisConnection != null && this._redisConnection.IsConnected)
            {
                return this._redisConnection;
            }
            //加锁，防止异步编程中，出现单例无效的问题
            lock (redisConnectionLock)
            {
                if (this._redisConnection != null)
                {
                    //释放redis连接
                    this._redisConnection.Dispose();
                }
                try
                {
                    var config = new ConfigurationOptions
                    {
                        AbortOnConnectFail = _redisCacheOptions.AbortOnConnectFail,
                        AllowAdmin = _redisCacheOptions.AllowAdmin,
                        ConnectTimeout = _redisCacheOptions.ConnectTimeout,//改成15s
                        SyncTimeout = _redisCacheOptions.SyncTimeout,
                        Password = _redisCacheOptions.Password,//Redis数据库密码
                        EndPoints = { }
                    };
                    _redisCacheOptions.ConnectAddress.Split(";").ToList().ForEach(
                       x =>
                       {
                           config.EndPoints.Add(x);
                       });

                    return ConnectionMultiplexer.Connect(config);
                }
                catch (Exception)
                {
                    throw new Exception("Redis服务未启用，请开启该服务，并且请注意端口号");
                }
            }
        }
        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            foreach (var endPoint in this.GetRedisConnection().GetEndPoints())
            {
                var server = this.GetRedisConnection().GetServer(endPoint);
                foreach (var key in server.Keys())
                {
                    _redisConnection.GetDatabase().KeyDelete(key);
                }
            }
        }
        /// <summary>
        /// 获取Redis连接对象
        /// </summary>
        /// <returns></returns>
        public ConnectionMultiplexer GetConnection()
        {
            return this._redisConnection;
        }
        /// <summary>
        /// 获取默认数据库
        /// </summary>
        /// <returns></returns>
        public IDatabase GetDatabase()
        {
            return this._redisConnection.GetDatabase();
        }
    }
}
