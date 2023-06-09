using IdentityServer.EFCore.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreStudy.First.Domain;
using NetCoreStudy.First.Domain.Entity;
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
        private readonly IUserManagerService _userManagerService;

        public UserController(UserManager<MyUser> userManager, CustomUserManager<MyUser> customUserManager, IUserManagerService userManagerService)
        {
            _userManager = userManager;
            _customUserManager = customUserManager;
            _userManagerService = userManagerService;
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

        /// <summary>
        /// 根据查询条件查询用户
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<List<MyUser>> getUsersByCondition(UserQueryCondition userQueryCondition)
        {
           
            var result = await _userManagerService.GetUsersByDynamicConditionAsync(userQueryCondition);
            return result;
        }
    }
}
