using IdentityServer.EFCore.Entity;
using Microsoft.EntityFrameworkCore;
using NetCoreStudy.First.Domain;
using NetCoreStudy.First.Domain.Entity;
using NetCoreStudy.First.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var user1 =await query.ToListAsync();
            return user1;
        }
    }
}
