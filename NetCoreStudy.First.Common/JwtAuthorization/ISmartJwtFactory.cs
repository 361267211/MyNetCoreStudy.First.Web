/*********************************************************
* 名    称：ISmartJwtFactory.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210827
* 描    述：定义获取token域刷新token的方法
* 更新历史：
*
* *******************************************************/
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace NetCoreStudy.Core.JwtAuthorization
{
    /// <summary>
    /// Jwt Token服务接口
    /// </summary>
    public interface ISmartJwtFactory
    {
        /// <summary>
        /// 通过Claims获取Token
        /// </summary>
        /// <param name="claims">用户信息</param>
        /// <param name="needRefreshToken">是否需要RefreshToken</param>
        /// <returns></returns>
        JwtResponseToken GenerateToken(IEnumerable<Claim> claims, bool needRefreshToken);
        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="oldAccessToken">需要刷新的Token</param>
        /// <returns></returns>
        JwtResponseToken RefreshToken(string oldAccessToken);

    }
}
