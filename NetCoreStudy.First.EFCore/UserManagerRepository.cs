using IdentityServer.EFCore.Entity;
using Microsoft.EntityFrameworkCore;
using NetCoreStudy.First.Domain;
using NetCoreStudy.First.Domain.FxDto;
using NetCoreStudy.First.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.EFCore
{
    public class UserManagerRepository : IUserManagerRepository
    {
        private readonly ApplicationDbContext _appUserDb;

        public UserManagerRepository(ApplicationDbContext appUserDb)
        {
            _appUserDb = appUserDb;
        }

        public async Task<List<MyUser>> GetUsersByDynamicConditionAsync(UserQueryCondition queryCondition)
        {
            var query =  _appUserDb.Users.AsQueryable();
            foreach (var condition in queryCondition.ConditionList)
            {
                query= query.CreateExp(condition.FieldName, condition.FieldValue);
            }
            //var user1 =await query.ToListAsync();

            List<MyUser> userlist = new List<MyUser>();
            userlist.Add(new MyUser { UserName = "666" });
            return userlist;
        }

        public async Task UpdateUser(MyUserDto userDto)
        {
            var user = await _appUserDb.Users.FirstAsync(e => e.UserName == userDto.UesrName);
            user.WeiChatAccount = "138***4813" + DateTime.Now.ToString();
            _appUserDb.Users.Update(user);


        }
    }
}
