/*********************************************************
* 名    称：IRedisCacheManager.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210831
* 描    述：Redis缓存管理接口
* 更新历史：
*
* *******************************************************/
using StackExchange.Redis;

namespace NetCoreStudy.Core.Redis
{
    /// <summary>
    /// Redis缓存接口
    /// </summary>
    public interface IRedisCacheManager
    {
        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        ConnectionMultiplexer GetConnection();
        /// <summary>
        /// 获取默认数据库
        /// </summary>
        /// <returns></returns>
        public IDatabase GetDatabase();
        /// <summary>
        /// 清除数据
        /// </summary>
        void Clear();
    }
}
