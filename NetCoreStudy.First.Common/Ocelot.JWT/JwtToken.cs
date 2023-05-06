using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartLibrary.Core.Ocelot.JWT
{

    public class JwtToken
    {
        /// <summary>
        /// 获取基于JWT的Token
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="permissionRequirement"></param>
        /// <param name="clientType"></param>
        /// <param name="TokenType">1.token   2 refreshToken</param>
        /// <returns></returns>
        public static dynamic BuildJwtToken(Claim[] claims, PermissionRequirement permissionRequirement, AuthClientType clientType)
        {
            double expiresIn = 2 * 60 * 60 * 1000;
            double refreshTokenExpiresIn = 24 * 60 * 60 * 1000;
            switch (clientType)
            {
                case AuthClientType.SaasPlatForm://SAAS后台 
                    refreshTokenExpiresIn = 8 * 60 * 60 * 1000; //设置成8小时  
                    break;
                case AuthClientType.ToCWehcatApp://小程序
                    refreshTokenExpiresIn = 7 * 24 * 60 * 60 * 1000; //设置成7天  用于小程序客户端之类的，能确保一周永久在线
                    break;
                default:
                    refreshTokenExpiresIn = 7 * 24 * 60 * 60 * 1000;
                    break;
            }

            var now = DateTime.Now;
            var jwt_token = new JwtSecurityToken(
            issuer: permissionRequirement.Issuer,
            audience: permissionRequirement.Audience,
            claims: claims,
            notBefore: now,
            expires: now.Add(TimeSpan.FromMilliseconds(expiresIn)),
            //expires: now.Add(TimeSpan.FromMilliseconds(5 * 60 * 1000)),
            signingCredentials: permissionRequirement.SigningCredentials
            );

            //用于refresh使用的 jwt
            var jwt_refreshtoken = new JwtSecurityToken(
            issuer: permissionRequirement.Issuer,
            audience: permissionRequirement.Audience,
            claims: claims,
            notBefore: now,
            expires: now.Add(TimeSpan.FromMilliseconds(refreshTokenExpiresIn)),
            //expires: now.Add(TimeSpan.FromMilliseconds(1000 * 60 * 10)),
            signingCredentials: permissionRequirement.SigningCredentials
            );

            var encodedJwt_Token = new JwtSecurityTokenHandler().WriteToken(jwt_token);
            var encodedJwt_RefreshToken = new JwtSecurityTokenHandler().WriteToken(jwt_refreshtoken);

            var responseTokenJson = new
            {
                access_token = encodedJwt_Token,
                expires = now.Add(TimeSpan.FromMilliseconds(expiresIn)),
                expires_in = expiresIn,
                //expires = now.Add(TimeSpan.FromMilliseconds(5 * 60 * 1000)),
                //expires_in = 5 * 60 * 1000,
                token_type = "Bearer",
                refresh_access_token = new
                {
                    refresh_token = encodedJwt_RefreshToken,
                    expires = now.Add(TimeSpan.FromMilliseconds(refreshTokenExpiresIn)),
                    expires_in = refreshTokenExpiresIn
                    //expires = now.Add(TimeSpan.FromMilliseconds(1000 * 60 * 10)),
                    //expires_in = 1000 * 60 * 10
                }
            };

            return responseTokenJson;
        }
    }
}
