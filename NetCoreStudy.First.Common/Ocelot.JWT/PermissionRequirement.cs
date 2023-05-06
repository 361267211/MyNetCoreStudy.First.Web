using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLibrary.Core.Ocelot.JWT
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 请求路径
        /// </summary>
        public string LoginPath { get; set; } = "/api/Auth/Login";
        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 订阅人
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 签名验证
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="deniedAction">无权限action</param>
        /// <param name="userPermissions">用户权限集合</param>

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="deniedAction">拒约请求的url</param> 
        /// <param name="claimType">声明类型</param>
        /// <param name="issuer">发行人</param>
        /// <param name="audience">订阅人</param>
        /// <param name="signingCredentials">签名验证实体</param>
        public PermissionRequirement(string issuer, string audience, SigningCredentials signingCredentials, string openJWT)
        {
            Issuer = issuer;
            Audience = audience;
            SigningCredentials = signingCredentials;
            OpenJWT = openJWT;
        }

        /// <summary>
        /// 是否启用jwt验证
        /// </summary>
        public string OpenJWT { get; set; }
    }
}
