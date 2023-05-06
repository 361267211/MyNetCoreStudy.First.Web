/*********************************************************
* 名    称：JwtResponseToken.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210831
* 描    述：Jwt授权返回结果
* 更新历史：
*
* *******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.Core.JwtAuthorization
{
    /// <summary>
    /// Token信息描述
    /// </summary>
    public class JwtResponseToken
    {
        /// <summary>
        /// Token值
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Token过期时间
        /// </summary>
        public DateTime TokenExpiredAt { get; set; }
        /// <summary>
        /// RefreshToken
        /// </summary>
        public string RefreshToken { get; set; }
        /// <summary>
        /// RefreshToken过期时间
        /// </summary>
        public DateTime? RefreshTokenExpireAt { get; set; }
    }

}
