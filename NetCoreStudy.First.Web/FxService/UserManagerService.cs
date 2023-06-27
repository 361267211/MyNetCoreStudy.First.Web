using IdentityServer.EFCore.Entity;
using NetCoreStudy.First.Domain.Entity;
using NetCoreStudy.First.Web.FxDto;
using NetCoreStudy.First.Web.FxRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Domain.FxService
{
    public class UserManagerService : IUserManagerService
    {
        private IUserManagerRepository _userManagerRepository;

        public UserManagerService(IUserManagerRepository userManagerRepository)
        {
            _userManagerRepository = userManagerRepository;
        }

        public async Task<List<MyUser>> GetUsersByDynamicConditionAsync(UserQueryCondition queryCondition)
        {
            return await _userManagerRepository.GetUsersByDynamicConditionAsync(queryCondition);
        }



        public async Task UpdateUser(MyUserDto userDto)
        {
            await _userManagerRepository.UpdateUser(userDto);
        }
    }
}
