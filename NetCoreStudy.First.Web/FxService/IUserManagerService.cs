using IdentityServer.EFCore.Entity;
using NetCoreStudy.First.Domain.Entity;
using NetCoreStudy.First.Web.FxDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Domain.FxService
{
    public interface IUserManagerService
    {
        Task<List<MyUser>> GetUsersByDynamicConditionAsync(UserQueryCondition queryCondition);
        Task UpdateUser(MyUserDto userDto);
    }
}
