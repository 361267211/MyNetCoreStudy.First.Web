using IdentityServer.EFCore.Entity;
using Microsoft.EntityFrameworkCore;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.Utility;
using NetCoreStudy.First.Web.FxAttribute;
using NetCoreStudy.First.Web.FxDto.FxUser;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxRepository.FxUser
{
    public class UserManagerRepository : IUserManagerRepository
    {
        private readonly ApplicationDbContext _appUserDb;

        public UserManagerRepository(ApplicationDbContext appUserDb)
        {
            _appUserDb = appUserDb;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryCondition"></param>
        /// <returns></returns>
        [Caching(nameof(MyUser))] //使用缓存AOP 缓存10分钟,资源名称
        public async Task<List<MyUser>> GetUsersByDynamicConditionAsync(UserQueryCondition queryCondition)
        {
            //模拟耗时操作
            Task.Delay(1000).Wait();


            var query = _appUserDb.Users.AsQueryable();
            foreach (var condition in queryCondition.ConditionList)
            {
                query = query.CreateExp(condition.FieldName, condition.FieldValue);
            }
            var userlist = await query.ToListAsync();

            return userlist;
        }
        [CachingChange(ResourceName = nameof(MyUser))]
        public async Task UpdateUser(MyUserDto userDto)
        {
            var user = await _appUserDb.Users.FirstAsync(e => e.UserName == userDto.UesrName);
            user.WeiChatAccount = "138***4813" + DateTime.Now.ToString();
            _appUserDb.Users.Update(user);

            return;
        }
    }
}
