/*********************************************************
* 名    称：SmartConsulOptions.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210826
* 描    述：Consul注册参数配置
* 更新历史：
*
* *******************************************************/

namespace NetCoreStudy.Core.Consul
{
    /// <summary>
    /// Consul注册配置信息
    /// </summary>
    public class SmartConsulOptions
    {
        /// <summary>
        /// Consul Agent地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Token信息
        /// </summary>
        public string Token { get; set; }
    }
}
