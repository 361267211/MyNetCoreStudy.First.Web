using IdentityServer.EFCore.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetCoreStudy.First.EFCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web
{
    public class CustomUserStore<TUser> : UserStore<TUser, MyRole, ApplicationDbContext, long>
         where TUser : MyUser
    {

        private readonly ApplicationDbContext _dbContext;
        public CustomUserStore(ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            _dbContext = context;
        }

        public async Task<List<TUser>> GetUserByCondition(string condition)
        {
            // 在这里实现根据自定义条件查询用户的逻辑
            // ...
            List<TUser> users = _dbContext.Users.Where(e => e.UserName == condition).ToList() as List<TUser>;
            // 返回符合条件的用户
            return users;
        }
    }
}
