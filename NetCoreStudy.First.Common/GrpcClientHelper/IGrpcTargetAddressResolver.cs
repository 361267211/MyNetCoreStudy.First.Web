/*********************************************************
* 名    称：IGrpcTargetAddressResolver.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210827
* 描    述：Grpc网关地址获取接口
* 更新历史：
*
* *******************************************************/

namespace NetCoreStudy.Core.GrpcClientHelper
{
    /// <summary>
    /// Grpc连接地址获取接口
    /// </summary>
    public interface IGrpcTargetAddressResolver
    {
        /// <summary>
        /// 通过OrgCode获取Grpc服务连接地址
        /// </summary>
        /// <param name="orgCode"></param>
        /// <returns></returns>
        public string GetGrpcTargetAddress(string orgCode);
    }
}
