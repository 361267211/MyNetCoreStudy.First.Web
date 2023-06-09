using IdentityServer.EFCore.Entity;
using NetCoreStudy.First.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Domain
{
     public interface IUserManagerRepository
    {
        Task<List<MyUser>> GetUsersByDynamicConditionAsync(UserQueryCondition queryCondition);
    }
}
