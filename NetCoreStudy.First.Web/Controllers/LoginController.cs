using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetCoreStudy.First.Web.JWT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IOptionsSnapshot<JWTSettings> _JwtSettingsOpt;

        public LoginController(IOptionsSnapshot<JWTSettings> JwtOption)
        {
            _JwtSettingsOpt = JwtOption;
        }
        [HttpPost]
        public ActionResult<string> Login(LoginRequest req)
        {
            if (req.UserName=="admin"&&req.Password=="123456")
            {
              //  var item = Process.GetProcesses().Select(p => new ProcessInfo(p.Id, p.ProcessName, p.WorkingSet64));

                //JWT
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim("Passport", "123456"));
                claims.Add(new Claim("QQ", "36126"));
                claims.Add(new Claim("Id", "666"));
                claims.Add(new Claim("Address", "chongqing"));
                claims.Add(new Claim(ClaimTypes.Role, "admin"));

                string key = "aaasssdddfffggghhhjjjkkklll";
                DateTime expire = DateTime.Now.AddHours(1);//过期时间点

                byte[] secBytes = Encoding.UTF8.GetBytes(_JwtSettingsOpt.Value.SecKey);
                var secKey = new SymmetricSecurityKey(secBytes);//密钥
                var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);//加密算法类型
                var tokenDescriptor = new JwtSecurityToken(claims: claims, expires:  DateTime.Now.AddSeconds(_JwtSettingsOpt.Value.ExpireSeconds), signingCredentials: credentials);
                string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

                return jwt;
            }
            else
            {
                return BadRequest();
            }
        }
    }

    public record LoginRequest(string UserName,string Password);
    public record ProcessInfo(long Id, string ProcessName,long WorkingSet);
    public record LoginReponse(bool Ok, ProcessInfo[] ProcessInfos);


}
