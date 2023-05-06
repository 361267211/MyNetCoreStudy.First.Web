/*********************************************************
* 名    称：SmartJwtOptions.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210827
* 描    述：Jwt授权配置信息
* 更新历史：
*
* *******************************************************/
using Microsoft.IdentityModel.Tokens;

namespace NetCoreStudy.Core.JwtAuthorization
{
    /// <summary>
    /// Jwt配置信息
    /// </summary>
    public class SmartJwtOptions
    {
        /// <summary>
        /// 颁发者
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 参与者
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 私钥
        /// </summary>
        public SecurityKey PrivateKey { get; set; }
        /// <summary>
        /// 公钥
        /// </summary>
        public SecurityKey PublicKey { get; set; }
        /// <summary>
        /// Token设置过期时间，单位秒，默认7200
        /// </summary>
        public double TokenExpireSecs { get; set; } = 7200;
        /// <summary>
        /// RefreshToken设置过期时间，单位秒，默认86400
        /// </summary>
        public double RefreshTokenExpireSecs { get; set; } = 86400;
    }
}
