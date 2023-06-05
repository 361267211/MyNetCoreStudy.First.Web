using IdentityServer.EFCore.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Controllers.UserManager
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly CustomUserManager<MyUser> _customUserManager;

        public UserController(UserManager<MyUser> userManager, CustomUserManager<MyUser> customUserManager)
        {
            _userManager = userManager;
            _customUserManager = customUserManager;
        }

        [HttpGet]
        public async Task<List<MyUser>> getAllUsers()
        {
            var result = await _customUserManager.GetUserByCondition("felix.zhang");
            return result;
        }

        [HttpPost]
        public async Task RegisterAccount()
        {
            var user = new MyUser
            {
                UserName = "felix.zhang",
                Email = "felix.zhang@pwc.com"
            };

            var password = "Pwcwelcome1";

            var result = await _customUserManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // 用户创建成功，并设置了密码
            }
            else
            {
                // 用户创建失败，处理错误
                var errors = result.Errors;
            }

        }
    }
}
