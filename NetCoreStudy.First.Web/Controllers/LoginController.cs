using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public LoginReponse Login(LoginRequest req)
        {
            if (req.UserName=="admin"&&req.Password=="123456")
            {
                var item = Process.GetProcesses().Select(p => new ProcessInfo(p.Id, p.ProcessName, p.WorkingSet64));
                return new LoginReponse(true, item.ToArray());
            }
            else
            {
                return new LoginReponse(true, null);
            }
        }
    }

    public record LoginRequest(string UserName,string Password);
    public record ProcessInfo(long Id, string ProcessName,long WorkingSet);
    public record LoginReponse(bool Ok, ProcessInfo[] ProcessInfos);


}
