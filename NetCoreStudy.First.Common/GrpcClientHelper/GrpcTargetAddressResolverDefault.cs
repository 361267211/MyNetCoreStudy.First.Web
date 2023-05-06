/*********************************************************
* 名    称：GrpcTargetAddressResolverDefault.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210827
* 描    述：通过机构编码获取机构grpc网关地址，默认服务，仅用于占位
* 更新历史：
*
* *******************************************************/

namespace NetCoreStudy.Core.GrpcClientHelper
{
    /// <summary>
    /// Grpc连接地址获取默认实现类
    /// </summary>
    public class GrpcTargetAddressResolverDefault : IGrpcTargetAddressResolver
    {
        /// <summary>
        /// 获取Grpc服务连接地址
        /// </summary>
        /// <param name="orgCode"></param>
        /// <returns></returns>
        public string GetGrpcTargetAddress(string orgCode)
        {
            return "http://192.168.21.46:9999";
        }
    }
}
