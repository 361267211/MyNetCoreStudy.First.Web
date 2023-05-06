/*********************************************************
* 名    称：SmartJwtFactory.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210827
* 描    述：提供Token获取和Token刷新功能
* 更新历史：
*
* *******************************************************/
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace NetCoreStudy.Core.JwtAuthorization
{
    /// <summary>
    /// Jwt Token服务实现类
    /// </summary>
    public class SmartJwtFactory : ISmartJwtFactory
    {
        /// <summary>
        /// 配置信息
        /// </summary>
        private readonly SmartJwtOptions _jwtOptions;
        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="jwtOptions"></param>
        public SmartJwtFactory(IOptions<SmartJwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        /// <summary>
        /// 通过Claims获取token信息
        /// </summary>
        /// <param name="claims">用户信息</param>
        /// <param name="needRefreshToken">是否需要RefreshToken</param>
        /// <returns></returns>
        public JwtResponseToken GenerateToken(IEnumerable<Claim> claims, bool needRefreshToken = false)
        {
            if (claims == null || !claims.Any())
            {
                throw new Exception("未找到有效身份信息");
            }
            var nowTime = DateTime.Now;
            var tokenExpireAt = DateTime.Now.AddSeconds(_jwtOptions.TokenExpireSecs);
            var refreshExpireAt = DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpireSecs);
            var encodedToken = "";
            var encodedRefreshToken = "";
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: nowTime,
                expires: tokenExpireAt,
                signingCredentials: new SigningCredentials(_jwtOptions.PrivateKey, SecurityAlgorithms.RsaSha256)
                );

            encodedToken = jwtTokenHandler.WriteToken(token);

            if (needRefreshToken)
            {
                var refreshClaims = new List<Claim> { new Claim("Role", "Refresh") };
                refreshClaims.AddRange(claims);

                var refreshToken = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: refreshClaims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpireSecs),
                signingCredentials: new SigningCredentials(_jwtOptions.PrivateKey, SecurityAlgorithms.RsaSha256)
                );
                encodedRefreshToken = jwtTokenHandler.WriteToken(refreshToken);
            }
            return new JwtResponseToken
            {
                Token = encodedToken,
                TokenExpiredAt = tokenExpireAt,
                RefreshToken = encodedRefreshToken,
                RefreshTokenExpireAt = !string.IsNullOrWhiteSpace(encodedRefreshToken) ? refreshExpireAt : null
            };
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="oldAccessToken">需要刷新的Token</param>
        /// <returns></returns>
        public JwtResponseToken RefreshToken(string oldAccessToken)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var canRead = jwtTokenHandler.CanReadToken(oldAccessToken);
            if (!canRead)
            {
                throw new Exception("传入访问令牌格式错误");
            }
            //定义校验规则
            var validateParameter = new TokenValidationParameters
            {
                ValidateAudience = true,
                // 验证发布者
                ValidateIssuer = true,
                // 验证过期时间
                ValidateLifetime = false,//不校验过期时间
                // 验证秘钥
                ValidateIssuerSigningKey = true,
                // 读配置Issure
                ValidIssuer = _jwtOptions.Issuer,
                // 读配置Audience
                ValidAudience = _jwtOptions.Audience,
                // 设置生成token的秘钥
                IssuerSigningKey = _jwtOptions.PublicKey
            };
            SecurityToken validatedToken = null;
            try
            {
                jwtTokenHandler.ValidateToken(oldAccessToken, validateParameter, out validatedToken);
            }
            catch (Exception ex)
            {
                throw new Exception("token校验未通过：" + ex.Message);
            }
            var oldJwtToken = validatedToken as JwtSecurityToken;//转换一下
            var claims = oldJwtToken.Claims;
            var newTokenResult = this.GenerateToken(claims, true);
            return newTokenResult;
        }
    }
}
