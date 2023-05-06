using IdentityServer.EFCore.Entity;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Zack.Commons;

namespace NetCoreStudy.First.Web.Controllers
{
    [Route("identity")]
     
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly RoleManager<MyRole> _roleManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;

        public IdentityController(UserManager<MyUser> userManager, RoleManager<MyRole> roleManager, IIdentityServerInteractionService interaction, IClientStore clientStore, IAuthenticationSchemeProvider schemeProvider, IEventService events)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
        }
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }


        [Route("AddRole")]
        [HttpGet]
        public string AddRole(string roleName)
        {
            if (_roleManager.FindByNameAsync(roleName).Result == null)
            {
                var role = new MyRole
                {
                    Name = roleName
                };
                var result = _roleManager.CreateAsync(role).Result;
            }
            return "success";
        }


        [Route("AddUser")]
        [HttpGet]
        public string AddUser(string userName)
        {
            if (_userManager.FindByNameAsync(userName).Result == null)
            {
                var user = new MyUser
                {
                    UserName=userName
                   
            };
                var result = _userManager.CreateAsync(user,"123456").Result;
            }
            return "success";
        }

        [Route("AddRolesWithUserName")]
        [HttpGet]
        public string AddRolesWithUserName(string userName, string roleNames)
        {
            var user = _userManager.FindByNameAsync(userName).Result;
            var allRoles = _roleManager.Roles.ToList(); //系统中所有的角色
            var nowRoles = _userManager.GetRolesAsync(user).Result; //该用户现有的角色“Name”
            foreach (var nowRole in nowRoles)
            {
                var normalizedName = allRoles.Where(r => r.Name == nowRole).First().NormalizedName;//取得现有角色的NormalizedName
                var deleteResult = _userManager.RemoveFromRoleAsync(user, normalizedName).Result; //删除不要的角色
            }
            var result = _userManager.AddToRolesAsync(user, roleNames.Split(',')).Result; //添加需要的角色
            _userManager.UpdateAsync(user); //完成储存
            return "success";
        }

    }
}
