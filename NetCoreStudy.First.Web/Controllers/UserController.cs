
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreStudy.First.Domain;
using NetCoreStudy.First.Domain.Entity;
using NetCoreStudy.First.Domain.ValueObj;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.Utility.DistributedCache;
using NetCoreStudy.First.Web.Request;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserDomainRepository _userRepository;
        private readonly UserDomainService _userService;
        private readonly IDistributedCacheHelper _distrCache;
        private readonly UserDbContext _userDbContext;

        public UserController(IUserDomainRepository userRepository, UserDomainService userService, IDistributedCacheHelper distrCache, UserDbContext userDbContext)
        {
            _userRepository = userRepository;
            _userService = userService;
            _distrCache = distrCache;
            _userDbContext = userDbContext;
        }

        [HttpPut]
      //  [UnitOfWork(typeof(UserDbContext))]
        public async Task<IActionResult> LoginByPhoneAndPwd(LoginByPhoneAndPwdRequest req)
        {
            if (req.password.Length <= 3)
            {
                return BadRequest("密码长度小于3");
            }
            var res = await _userService.CheckPassword(phoneNumber: req.PhoneNumber, req.password);

            switch (res)
            {
                case UserAccessResult.OK:
                    return Ok("登录成功");
                case UserAccessResult.Lockout:
                    return BadRequest("账号被锁定");
                default:
                    return BadRequest("登录失败");
            }
        
        }

        [HttpPut]
        [UnitOfWork(typeof(UserDbContext))]
        public async Task<IActionResult> AddUser(AddUserRequest req)
        {
            if ((await _userRepository.FindOneAsync(req.phoneNo)) != null)
            {
                return BadRequest("账号重复");
            }
            var user=new User(req.phoneNo);
            user.ChangePassword(req.password);
            _userDbContext.Users.Add(user);
            return Ok();
        }
    }
}
