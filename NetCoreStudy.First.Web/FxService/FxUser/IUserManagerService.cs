using IdentityServer.EFCore.Entity;
using NetCoreStudy.First.Domain.Entity;
using NetCoreStudy.First.Web.FxDto.FxUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxRepository.FxUser
{
    public interface IUserManagerService
    {
        Task<List<MyUser>> GetUsersByDynamicConditionAsync(UserQueryCondition queryCondition);
        Task UpdateUser(MyUserDto userDto);
    }
}
