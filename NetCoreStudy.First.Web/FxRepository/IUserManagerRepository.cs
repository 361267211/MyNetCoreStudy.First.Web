using IdentityServer.EFCore.Entity;
using NetCoreStudy.First.Domain.Entity;
using NetCoreStudy.First.Web.FxDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxRepository
{
    public interface IUserManagerRepository
    {
        Task<List<MyUser>> GetUsersByDynamicConditionAsync(UserQueryCondition queryCondition);
        Task<long> UpdateUser(MyUserDto userDto);
    }
}
