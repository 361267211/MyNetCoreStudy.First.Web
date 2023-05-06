/*********************************************************
* 名    称：RedisCacheOptions.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210901
* 描    述：******
* 更新历史：
*
* *******************************************************/
namespace NetCoreStudy.Core.Redis
{
    public class RedisCacheOptions
    {
        public bool AbortOnConnectFail { get; set; }
        public bool AllowAdmin { get; set; }
        public int ConnectTimeout { get; set; }
        public int SyncTimeout { get; set; }
        public string Password { get; set; }
        public string ConnectAddress { get; set; }
    }
}
