
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreStudy.First.Domain.Entity;
using NetCoreStudy.First.Domain.ValueObj;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.Utility.DistributedCache;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly TestDbContext _dbContext;
        private readonly UserDomainRepository _userRepository;
        private readonly IDistributedCacheHelper _distrCache;

        public UserController(TestDbContext dbContext, IDistributedCacheHelper distrCache, UserDomainRepository userRepository)
        {
            _dbContext = dbContext;
            _distrCache = distrCache;
            _userRepository = userRepository;
        }

        [HttpGet]   
        public async Task GetUserByPhoneNumber(PhoneNumber phoneNumber)
        {
           User? user= await _userRepository.FindOneAsync(phoneNumber);
        }
    }
}
